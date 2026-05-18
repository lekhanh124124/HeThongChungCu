using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Commands.SyncKnowledgeBase;

public class SyncKnowledgeBaseCommandHandler : ICommandHandler<SyncKnowledgeBaseCommand, SyncResultDto>
{
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly ITextChunker _textChunker;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncKnowledgeBaseCommandHandler> _logger;

    public SyncKnowledgeBaseCommandHandler(
        IVectorStore vectorStore,
        IEmbeddingService embeddingService,
        ITextChunker textChunker,
        IConfiguration configuration,
        ILogger<SyncKnowledgeBaseCommandHandler> logger)
    {
        _vectorStore = vectorStore;
        _embeddingService = embeddingService;
        _textChunker = textChunker;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<SyncResultDto>> Handle(SyncKnowledgeBaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting knowledge base sync pipeline...");

            // 1. Xác định đường dẫn thư mục tri thức
            var kbPath = _configuration["KnowledgeBase:Path"];
            if (string.IsNullOrWhiteSpace(kbPath))
            {
                // Tự động tìm kiếm thư mục 'knowledge-base' bằng cách leo cây thư mục
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                while (!string.IsNullOrEmpty(currentDir))
                {
                    var testPath = Path.Combine(currentDir, "knowledge-base");
                    if (Directory.Exists(testPath))
                    {
                        kbPath = testPath;
                        break;
                    }
                    var parent = Path.GetDirectoryName(currentDir);
                    if (parent == currentDir) break; // Root directory reached
                    currentDir = parent;
                }
            }

            if (string.IsNullOrWhiteSpace(kbPath) || !Directory.Exists(kbPath))
            {
                _logger.LogError("Knowledge base directory not found. Configured path: {Path}", kbPath);
                return Result.Failure<SyncResultDto>(new Error("AI.KnowledgeBaseNotFound", "Không tìm thấy thư mục cơ sở tri thức 'knowledge-base'."));
            }

            _logger.LogInformation("Resolved knowledge-base path: {Path}", kbPath);

            // 2. Đảm bảo collection Qdrant tồn tại
            var modelId = _embeddingService.ModelId;
            var collectionName = _configuration[$"Qdrant:Collections:{modelId}"];
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = _configuration["Qdrant:CollectionName"] ?? "resident_knowledge_base";
            }

            var vectorSizeStr = _configuration["Gemini:EmbeddingVectorSize"] ?? "3072";
            if (!ulong.TryParse(vectorSizeStr, out var vectorSize))
            {
                vectorSize = 3072;
            }

            if (request.ForceRebuild)
            {
                _logger.LogInformation("ForceRebuild is true. Dropping Qdrant collection '{CollectionName}'...", collectionName);
                await _vectorStore.DeleteCollectionAsync(collectionName, cancellationToken);
            }

            _logger.LogInformation("Ensuring Qdrant collection '{CollectionName}' exists (Dimension: {VectorSize})...", collectionName, vectorSize);
            await _vectorStore.CreateCollectionIfNotExistsAsync(collectionName, vectorSize, cancellationToken);

            // Tạo các chỉ mục payload cho các trường metadata dùng để lọc dữ liệu
            _logger.LogInformation("Creating payload indices for keyword filtering...");
            await _vectorStore.CreatePayloadIndexAsync(collectionName, "document_type", "keyword", cancellationToken);
            await _vectorStore.CreatePayloadIndexAsync(collectionName, "source", "keyword", cancellationToken);

            // 3. Quét tất cả các tệp Markdown (*.md) trong thư mục
            var files = Directory.GetFiles(kbPath, "*.md", SearchOption.AllDirectories);
            _logger.LogInformation("Found {Count} Markdown files in directory.", files.Length);

            if (request.MaxFilesToSync.HasValue && request.MaxFilesToSync.Value > 0)
            {
                files = files.Take(request.MaxFilesToSync.Value).ToArray();
                _logger.LogInformation("Test mode: Limiting sync to {Count} files.", files.Length);
            }

            var totalProcessedFiles = 0;
            var totalChunksCount = 0;
            var allRecords = new List<VectorRecord>();

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relativePath = Path.GetRelativePath(kbPath, file).Replace("\\", "/");
                var sourceName = Path.GetFileName(file);

                // Lấy thư mục cha làm phân loại tài liệu (ví dụ: '11-thong-tin-lien-he')
                var documentType = Path.GetDirectoryName(relativePath)?.Replace("\\", "/") ?? "Chung";

                _logger.LogDebug("Processing file: {RelativePath} (Type: {Type})", relativePath, documentType);

                var content = await File.ReadAllTextAsync(file, System.Text.Encoding.UTF8, cancellationToken);
                if (string.IsNullOrWhiteSpace(content)) continue;

                // Chia nhỏ văn bản thành các đoạn thông qua ITextChunker
                var chunks = _textChunker.SplitText(content, sourceName, chunkSize: 400, chunkOverlap: 60);
                if (chunks == null || !chunks.Any()) continue;

                // Xóa toàn bộ chunk cũ của file này trước khi nạp lại để tránh chunk mồ côi (orphaned chunks)
                // khi file bị rút ngắn (số chunk mới ít hơn số chunk cũ).
                _logger.LogDebug("Deleting old chunks for file: {RelativePath}...", relativePath);
                await _vectorStore.DeleteBySourceAsync(collectionName, relativePath, cancellationToken);


                foreach (var chunk in chunks)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Sinh vector embedding cho đoạn văn bản
                    var vector = await _embeddingService.GenerateEmbeddingAsync(chunk.Content, cancellationToken);

                    // Trì hoãn 650ms để tôn trọng hạn ngạch Gemini Free Tier 100 RPM (trung bình 1 request mỗi 600-700ms)
                    await Task.Delay(650, cancellationToken);

                    // Sử dụng deterministic ID bằng cách kết hợp đường dẫn tương đối và chỉ mục đoạn
                    var deterministicId = $"{relativePath}#chunk-{chunk.ChunkIndex}";

                    var record = new VectorRecord
                    {
                        Id = deterministicId,
                        Vector = vector,
                        Payload = new Dictionary<string, object>
                        {
                            { "document_id", relativePath },
                            { "chunk_id", chunk.ChunkId },
                            { "source", relativePath },
                            { "content", chunk.Content },
                            { "h1", chunk.H1 ?? string.Empty },
                            { "h2", chunk.H2 ?? string.Empty },
                            { "h3", chunk.H3 ?? string.Empty },
                            { "document_type", documentType },
                            { "chunk_index", chunk.ChunkIndex },
                            { "token_count", chunk.TokenCount }
                        }
                    };

                    allRecords.Add(record);
                }

                totalProcessedFiles++;
            }

            // 4. Nạp dữ liệu vào Qdrant hàng loạt (Batch Upsert) với lô kích thước 50
            if (allRecords.Any())
            {
                var batchSize = 50;
                _logger.LogInformation("Upserting {Total} chunks to Qdrant in batches of {BatchSize}...", allRecords.Count, batchSize);

                for (int i = 0; i < allRecords.Count; i += batchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = allRecords.Skip(i).Take(batchSize).ToList();
                    await _vectorStore.UpsertVectorsBatchAsync(collectionName, batch, cancellationToken);
                    totalChunksCount += batch.Count;

                    _logger.LogDebug("Successfully upserted batch ({Start} to {End})", i, i + batch.Count - 1);
                }
            }

            _logger.LogInformation("Knowledge base sync completed successfully! Processed {FilesCount} files and upserted {ChunksCount} chunks.", totalProcessedFiles, totalChunksCount);

            return Result.Success(new SyncResultDto
            {
                TotalFilesProcessed = totalProcessedFiles,
                TotalChunksIngested = totalChunksCount,
                CollectionName = collectionName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during knowledge base sync pipeline.");
            return Result.Failure<SyncResultDto>(new Error("AI.SyncError", $"Lỗi đồng bộ tri thức: {ex.Message}"));
        }
    }
}
