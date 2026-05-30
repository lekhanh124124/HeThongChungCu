using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HeThongChungCu.Application.Features.AI.Commands.TestBatchSearch;

public class TestBatchSearchCommandHandler : ICommandHandler<TestBatchSearchCommand, TestBatchSearchResultDto>
{
    private const string EvenStatusIndexField = "even_status";

    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestBatchSearchCommandHandler> _logger;

    public TestBatchSearchCommandHandler(
        IVectorStore vectorStore,
        IEmbeddingService embeddingService,
        IConfiguration configuration,
        ILogger<TestBatchSearchCommandHandler> logger)
    {
        _vectorStore = vectorStore;
        _embeddingService = embeddingService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<TestBatchSearchResultDto>> Handle(TestBatchSearchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Starting batch upsert & search test on collection '{Collection}' with {Count} texts.",
                request.CollectionName, request.Texts.Count);

            // 1. Đảm bảo collection tồn tại (Dùng dimension động của active model)
            var aiProvider = _configuration["AI:Provider"] ?? "OpenAI";
            var isOpenAI = aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase);
            var vectorSizeKey = isOpenAI ? "OpenAI:EmbeddingVectorSize" : "Gemini:EmbeddingVectorSize";
            var defaultSize = isOpenAI ? 1536UL : 3072UL;
            
            var vectorSizeStr = _configuration[vectorSizeKey];
            if (!ulong.TryParse(vectorSizeStr, out var vectorSize))
            {
                vectorSize = defaultSize;
            }

            await _vectorStore.CreateCollectionIfNotExistsAsync(request.CollectionName, vectorSize, cancellationToken);

            // Đăng ký trước chỉ mục siêu dữ liệu (Payload Index) – keyword index cho bộ lọc chẵn/lẻ
            await _vectorStore.CreatePayloadIndexAsync(request.CollectionName, EvenStatusIndexField, "keyword", cancellationToken);

            // 2. Sinh embedding và build danh sách VectorRecord
            var records = new List<VectorRecord>();
            for (int i = 0; i < request.Texts.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var text = request.Texts[i];
                var vector = await _embeddingService.GenerateEmbeddingAsync(text, cancellationToken);

                records.Add(new VectorRecord
                {
                    Id = $"test-item-{i}",
                    Vector = vector,
                    Payload = new Dictionary<string, object>
                    {
                        { "document_id", "doc-test-100" },
                        { "chunk_id", $"chunk-test-{i}" },
                        { "source", "Resident_Manual_2026.md" },
                        { "category", "Regulation" },
                        { "title", "Hướng dẫn kiểm thử tích hợp RAG" },
                        { "content", text },
                        { "index", i },
                        { EvenStatusIndexField, (i % 2 == 0) ? "even" : "odd" }
                    }
                });
            }

            // 3. Thực hiện nạp hàng loạt (Batch Upsert)
            await _vectorStore.UpsertVectorsBatchAsync(request.CollectionName, records, cancellationToken);

            _logger.LogInformation("Batch upsert completed: {Count} records.", records.Count);

            // 4. Thực hiện tìm kiếm lấy phần tử đầu tiên làm truy vấn
            var queryVector = records.First().Vector;

            var searchWithoutFilter = await _vectorStore.SearchSimilarAsync(
                collectionName: request.CollectionName,
                queryVector: queryVector,
                limit: 5,
                cancellationToken: cancellationToken);

            var searchWithFilter = await _vectorStore.SearchSimilarAsync(
                collectionName: request.CollectionName,
                queryVector: queryVector,
                limit: 5,
                filterMetadata: new Dictionary<string, object> { { EvenStatusIndexField, "even" } },
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Search completed. Without filter: {WF} results, With filter: {F} results.",
                searchWithoutFilter?.Count ?? 0, searchWithFilter?.Count ?? 0);

            return Result.Success(new TestBatchSearchResultDto
            {
                Message = "Thử nghiệm Batch Upsert và Filtered Search thành công!",
                TotalUpserted = records.Count,
                SearchWithoutFilter = searchWithoutFilter ?? new(),
                SearchWithFilter = searchWithFilter ?? new()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch search test failed on collection '{Collection}'.", request.CollectionName);
            return Result.Failure<TestBatchSearchResultDto>(new Error("AI.BatchSearch.Error", $"Thử nghiệm Batch Search thất bại: {ex.Message}"));
        }
    }
}
