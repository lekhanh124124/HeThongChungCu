using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.SyncTriThucChatbot;

/// <summary>
/// Đồng bộ tri thức chatbot từ SQL DB lên Qdrant. Logic:
///
///   UPSERT → tất cả IsActive = true (idempotent, kể cả đã sync trước đó)
///   DELETE → IsActive = false AND IsSynced = true (từng có trong Qdrant, nay cần xóa)
///   SKIP   → IsActive = false AND IsSynced = false (chưa bao giờ lên Qdrant)
///
/// Idempotent: gọi nhiều lần cho cùng kết quả.
/// </summary>
public class SyncTriThucChatbotCommandHandler
    : ICommandHandler<SyncTriThucChatbotCommand, SyncTriThucChatbotResultDto>
{
    private const int EmbedConcurrency = 5;

    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly ITextChunker _textChunker;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncTriThucChatbotCommandHandler> _logger;

    public SyncTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork,
        IEmbeddingService embeddingService,
        IVectorStore vectorStore,
        ITextChunker textChunker,
        IConfiguration configuration,
        ILogger<SyncTriThucChatbotCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
        _textChunker = textChunker;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<SyncTriThucChatbotResultDto>> Handle(
        SyncTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var modelId = _embeddingService.ModelId;
        var collectionName = _configuration[$"Qdrant:Collections:{modelId}"];
        // var collectionName = "tri_thuc_chatbot_test_openai";
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            collectionName = _configuration["Qdrant:CollectionName"] ?? "resident_knowledge_base";
        }

        var aiProvider = _configuration["AI:Provider"] ?? "OpenAI";
        var isOpenAI = aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase);
        var vectorSizeKey = isOpenAI ? "OpenAI:EmbeddingVectorSize" : "Gemini:EmbeddingVectorSize";
        var defaultSize = isOpenAI ? "1536" : "3072";

        var vectorSizeStr = _configuration[vectorSizeKey] ?? defaultSize;
        if (!ulong.TryParse(vectorSizeStr, out var vectorSize))
        {
            vectorSize = isOpenAI ? 1536UL : 3072UL;
        }

        await _vectorStore.CreateCollectionIfNotExistsAsync(collectionName, vectorSize, cancellationToken);

        // Tạo các chỉ mục payload cho các trường metadata dùng để lọc dữ liệu
        await _vectorStore.CreatePayloadIndexAsync(collectionName, "document_type", "keyword", cancellationToken);
        await _vectorStore.CreatePayloadIndexAsync(collectionName, "source", "keyword", cancellationToken);

        // ── 1. Lấy 2 tập bản ghi cần xử lý ─────────────────────────────────
        var activeRecords = await _repository.GetAllActiveAsync(cancellationToken);
        var syncedInactiveRecords = await _repository.GetSyncedInactiveAsync(cancellationToken);

        _logger.LogInformation(
            "SyncTriThucChatbot: Active={Active}, SyncedInactive={Inactive}",
            activeRecords.Count, syncedInactiveRecords.Count);

        if (activeRecords.Count == 0 && syncedInactiveRecords.Count == 0)
        {
            _logger.LogInformation("SyncTriThucChatbot: Không có gì cần đồng bộ.");
            return Result.Success(new SyncTriThucChatbotResultDto { ElapsedMs = sw.ElapsedMilliseconds });
        }

        var errorIds = new ConcurrentBag<int>();
        var now = DateTimeOffset.UtcNow;

        // ── 2. UPSERT: embed song song → batch upsert 1 lần ─────────────────
        int upserted = 0;
        if (activeRecords.Count > 0)
        {
            var vectorRecords = await EmbedParallelAsync(activeRecords, collectionName, errorIds, cancellationToken);

            if (vectorRecords.Count > 0)
            {
                await _vectorStore.UpsertVectorsBatchAsync(collectionName, vectorRecords, cancellationToken);
                upserted = vectorRecords.Count;
                _logger.LogInformation("SyncTriThucChatbot: Upserted {Count} vectors.", upserted);
            }
        }

        // ── 3. DELETE: xóa bản ghi inactive đã từng sync ────────────────────
        int deleted = 0;
        if (syncedInactiveRecords.Count > 0)
        {
            deleted = await DeleteParallelAsync(syncedInactiveRecords, collectionName, errorIds, cancellationToken);
        }

        // ── 4. Cập nhật IsSynced ─────────────────────────────────────────────
        var failedIds = errorIds.ToHashSet();

        // Active thành công → IsSynced = true
        foreach (var r in activeRecords.Where(r => !failedIds.Contains(r.Id)))
            r.MarkAsSynced(now);

        // Inactive đã xóa thành công → IsSynced = false (không còn trong Qdrant)
        foreach (var r in syncedInactiveRecords.Where(r => !failedIds.Contains(r.Id)))
            r.MarkAsUnsynced();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        sw.Stop();

        _logger.LogInformation(
            "SyncTriThucChatbot hoàn tất: Upserted={U}, Deleted={D}, Errors={E}, Time={T}ms",
            upserted, deleted, failedIds.Count, sw.ElapsedMilliseconds);

        return Result.Success(new SyncTriThucChatbotResultDto
        {
            UpsertedCount = upserted,
            DeletedCount = deleted,
            SkippedCount = 0,      // không còn khái niệm "skip" trong logic mới
            ErrorIds = [.. failedIds],
            ElapsedMs = sw.ElapsedMilliseconds
        });
    }

    // ─── Private helpers ─────────────────────────────────────────────────────

    private async Task<List<VectorRecord>> EmbedParallelAsync(
        IReadOnlyList<TriThucChatbot> activeRecords,
        string collectionName,
        ConcurrentBag<int> errorIds,
        CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(EmbedConcurrency, EmbedConcurrency);
        var resultBag = new ConcurrentBag<VectorRecord>();

        var aiProvider = _configuration["AI:Provider"] ?? "OpenAI";
        var isGemini = aiProvider.Equals("Gemini", StringComparison.OrdinalIgnoreCase);

        var tasks = activeRecords.Select(async triThuc =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var sourceId = VectorId(triThuc.Id);

                // 1. Xóa toàn bộ chunk cũ của record này trước khi nạp lại để tránh chunk mồ côi
                await _vectorStore.DeleteBySourceAsync(collectionName, sourceId, cancellationToken);

                // 2. Chia nhỏ văn bản thành các đoạn thông qua ITextChunker
                var markdownContent = $"# {triThuc.TieuDe}\n\n{triThuc.NoiDung}";
                var chunks = _textChunker.SplitText(markdownContent, triThuc.TieuDe, chunkSize: 400, chunkOverlap: 60);
                if (chunks == null || !chunks.Any())
                {
                    _logger.LogWarning("SplitText return empty chunks for TriThuc ID={Id}", triThuc.Id);
                    return;
                }

                // 3. Sinh vector embedding cho từng đoạn
                foreach (var chunk in chunks)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var vector = await _embeddingService.GenerateEmbeddingAsync(chunk.Content, cancellationToken);

                    // Trì hoãn nếu dùng Gemini (Free Tier Rate Limit)
                    if (isGemini)
                    {
                        await Task.Delay(650, cancellationToken);
                    }

                    var deterministicId = $"{sourceId}#chunk-{chunk.ChunkIndex}";

                    resultBag.Add(new VectorRecord
                    {
                        Id = deterministicId,
                        Vector = vector,
                        Payload = new Dictionary<string, object>
                        {
                            { "document_id", sourceId },
                            { "chunk_id", chunk.ChunkId },
                            { "source", sourceId },
                            { "content", chunk.Content },
                            { "h1", chunk.H1 ?? string.Empty },
                            { "h2", chunk.H2 ?? string.Empty },
                            { "h3", chunk.H3 ?? string.Empty },
                            { "document_type", triThuc.DanhMuc },
                            { "chunk_index", chunk.ChunkIndex },
                            { "token_count", chunk.TokenCount },
                            { "db_id", triThuc.Id },
                            { "tieu_de", triThuc.TieuDe }
                        }
                    });
                }

                _logger.LogDebug("Embedded: ID={Id}, TieuDe={TieuDe}, ChunksCount={Count}", triThuc.Id, triThuc.TieuDe, chunks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi embed TriThucChatbot ID={Id}", triThuc.Id);
                errorIds.Add(triThuc.Id);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return [.. resultBag];
    }

    private async Task<int> DeleteParallelAsync(
        IReadOnlyList<TriThucChatbot> inactiveRecords,
        string collectionName,
        ConcurrentBag<int> errorIds,
        CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(EmbedConcurrency, EmbedConcurrency);
        int deleted = 0;

        var tasks = inactiveRecords.Select(async triThuc =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await _vectorStore.DeleteBySourceAsync(collectionName, VectorId(triThuc.Id), cancellationToken);
                Interlocked.Increment(ref deleted);
                _logger.LogDebug("Deleted from Qdrant: ID={Id}", triThuc.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi delete Qdrant TriThucChatbot ID={Id}", triThuc.Id);
                errorIds.Add(triThuc.Id);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return deleted;
    }

    private static string VectorId(int id) => $"db:tri-thuc-chatbot:{id}";
}
