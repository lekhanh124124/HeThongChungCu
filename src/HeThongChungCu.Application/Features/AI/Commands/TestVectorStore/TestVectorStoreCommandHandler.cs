using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HeThongChungCu.Application.Features.AI.Commands.TestVectorStore;

public class TestVectorStoreCommandHandler : ICommandHandler<TestVectorStoreCommand, TestVectorStoreResultDto>
{
    private const string TestCollectionName = "test_connection_collection";

    private readonly IVectorStore _vectorStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestVectorStoreCommandHandler> _logger;

    public TestVectorStoreCommandHandler(
        IVectorStore vectorStore,
        IConfiguration configuration,
        ILogger<TestVectorStoreCommandHandler> logger)
    {
        _vectorStore = vectorStore;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<TestVectorStoreResultDto>> Handle(TestVectorStoreCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Testing VectorStore connection by ensuring collection '{Collection}' exists.", TestCollectionName);

            var aiProvider = _configuration["AI:Provider"] ?? "OpenAI";
            var isOpenAI = aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase);
            var vectorSizeKey = isOpenAI ? "OpenAI:EmbeddingVectorSize" : "Gemini:EmbeddingVectorSize";
            var defaultSize = isOpenAI ? 1536UL : 3072UL;
            
            var vectorSizeStr = _configuration[vectorSizeKey];
            if (!ulong.TryParse(vectorSizeStr, out var vectorSize))
            {
                vectorSize = defaultSize;
            }

            await _vectorStore.CreateCollectionIfNotExistsAsync(TestCollectionName, vectorSize, cancellationToken);

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
