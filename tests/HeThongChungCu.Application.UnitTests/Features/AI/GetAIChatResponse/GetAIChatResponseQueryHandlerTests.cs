using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.AI.GetAIChatResponse;

public sealed class GetAIChatResponseQueryHandlerTests : BaseTest
{
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILLMService _llmService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetAIChatResponseQueryHandler> _logger;
    private readonly IChatbotContextEnricher _contextEnricher;
    private readonly GetAIChatResponseQueryHandler _handler;

    public GetAIChatResponseQueryHandlerTests()
    {
        _vectorStore = Substitute.For<IVectorStore>();
        _embeddingService = Substitute.For<IEmbeddingService>();
        _llmService = Substitute.For<ILLMService>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<GetAIChatResponseQueryHandler>>();
        _contextEnricher = Substitute.For<IChatbotContextEnricher>();

        _contextEnricher.EnrichAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(string.Empty));

        _handler = new GetAIChatResponseQueryHandler(
            _vectorStore,
            _embeddingService,
            _llmService,
            _configuration,
            _logger,
            _contextEnricher);
    }

    [Fact]
    public async Task Handle_Should_ResolveCollectionNameFromMapping_When_ModelIdConfigured()
    {
        // Arrange
        var modelId = "models/gemini-embedding-2";
        var expectedCollection = "resident_knowledge_base_gemini_2";

        _embeddingService.ModelId.Returns(modelId);
        _configuration[$"Qdrant:Collections:{modelId}"].Returns(expectedCollection);

        var query = new GetAIChatResponseQuery
        {
            Prompt = "Quy định gửi xe chung cư như thế nào?",
            Limit = 3
        };

        var mockEmbedding = new float[3072];
        _embeddingService.GenerateEmbeddingAsync(query.Prompt, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockEmbedding));

        var mockSearchResults = new List<VectorSearchResult>
        {
            new VectorSearchResult
            {
                Id = "result-1",
                Score = 0.95f,
                Payload = new Dictionary<string, object>
                {
                    { "source", "quy-dinh.md" },
                    { "content", "Phí gửi xe máy là 100k/tháng." },
                    { "h1", "Quy định gửi xe" }
                }
            }
        };

        _vectorStore.SearchSimilarAsync(
            collectionName: expectedCollection,
            queryVector: mockEmbedding,
            limit: query.Limit,
            filterMetadata: Arg.Any<Dictionary<string, object>?>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockSearchResults));

        _llmService.GenerateResponseAsync(
            prompt: query.Prompt,
            context: Arg.Any<string>(),
            systemInstruction: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("Phí gửi xe máy tại chung cư là 100.000 VNĐ mỗi tháng."));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Answer.Should().Be("Phí gửi xe máy tại chung cư là 100.000 VNĐ mỗi tháng.");
        result.Value.Sources.Should().HaveCount(1);
        result.Value.Sources[0].Source.Should().Be("quy-dinh.md");
        result.Value.Sources[0].Score.Should().Be(0.95f);

        // Verify SearchSimilarAsync was called with the mapped collection
        await _vectorStore.Received(1).SearchSimilarAsync(
            collectionName: expectedCollection,
            queryVector: mockEmbedding,
            limit: query.Limit,
            filterMetadata: null,
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_FallbackToDefaultCollection_When_NoConfiguredMapping()
    {
        // Arrange
        var modelId = "some-unknown-model";
        _embeddingService.ModelId.Returns(modelId);
        _configuration[$"Qdrant:Collections:{modelId}"].Returns((string?)null);
        _configuration["Qdrant:CollectionName"].Returns((string?)null); // Fallback to resident_knowledge_base

        var query = new GetAIChatResponseQuery
        {
            Prompt = "Quy định gửi xe chung cư như thế nào?",
            Limit = 3
        };

        var mockEmbedding = new float[3072];
        _embeddingService.GenerateEmbeddingAsync(query.Prompt, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockEmbedding));

        var mockSearchResults = new List<VectorSearchResult>();

        _vectorStore.SearchSimilarAsync(
            collectionName: "resident_knowledge_base",
            queryVector: mockEmbedding,
            limit: query.Limit,
            filterMetadata: Arg.Any<Dictionary<string, object>?>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockSearchResults));

        _llmService.GenerateResponseAsync(
            prompt: query.Prompt,
            context: Arg.Any<string>(),
            systemInstruction: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("Không tìm thấy thông tin phù hợp."));

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Answer.Should().Be("Không tìm thấy thông tin phù hợp.");

        // Verify SearchSimilarAsync was called with the fallback collection
        await _vectorStore.Received(1).SearchSimilarAsync(
            collectionName: "resident_knowledge_base",
            queryVector: mockEmbedding,
            limit: query.Limit,
            filterMetadata: null,
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
