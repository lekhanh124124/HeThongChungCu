using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Commands.TestVectorStore;

public class TestVectorStoreCommandHandler : ICommandHandler<TestVectorStoreCommand, TestVectorStoreResultDto>
{
    private const string TestCollectionName = "test_connection_collection";
    // Kích thước vector Gemini Embedding (text-embedding-004)
    private const ulong EmbeddingDimension = 3072;

    private readonly IVectorStore _vectorStore;
    private readonly ILogger<TestVectorStoreCommandHandler> _logger;

    public TestVectorStoreCommandHandler(IVectorStore vectorStore, ILogger<TestVectorStoreCommandHandler> logger)
    {
        _vectorStore = vectorStore;
        _logger = logger;
    }

    public async Task<Result<TestVectorStoreResultDto>> Handle(TestVectorStoreCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Testing VectorStore connection by ensuring collection '{Collection}' exists.", TestCollectionName);

            await _vectorStore.CreateCollectionIfNotExistsAsync(TestCollectionName, EmbeddingDimension, cancellationToken);

            _logger.LogInformation("VectorStore connection test successful.");

            return Result.Success(new TestVectorStoreResultDto
            {
                Message = "Kết nối Qdrant thành công qua IVectorStore wrapper!",
                CollectionChecked = TestCollectionName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "VectorStore connection test failed.");
            return Result.Failure<TestVectorStoreResultDto>(new Error("AI.VectorStore.ConnectionError", $"Kết nối Qdrant thất bại: {ex.Message}"));
        }
    }
}
