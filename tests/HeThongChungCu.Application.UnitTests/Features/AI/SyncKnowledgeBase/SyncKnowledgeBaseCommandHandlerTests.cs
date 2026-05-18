using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.AI.Commands.SyncKnowledgeBase;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.AI.SyncKnowledgeBase;

public sealed class SyncKnowledgeBaseCommandHandlerTests : BaseTest, IDisposable
{
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly ITextChunker _textChunker;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncKnowledgeBaseCommandHandler> _logger;
    private readonly SyncKnowledgeBaseCommandHandler _handler;
    private readonly string _tempKbPath;

    public SyncKnowledgeBaseCommandHandlerTests()
    {
        _vectorStore = Substitute.For<IVectorStore>();
        _embeddingService = Substitute.For<IEmbeddingService>();
        _textChunker = Substitute.For<ITextChunker>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<SyncKnowledgeBaseCommandHandler>>();

        // Create temporary knowledge-base folder and a dummy markdown file
        _tempKbPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempKbPath);
        File.WriteAllText(Path.Combine(_tempKbPath, "test.md"), "# Tiêu đề\nNội dung tri thức");

        _configuration["KnowledgeBase:Path"].Returns(_tempKbPath);
        _configuration["Gemini:EmbeddingVectorSize"].Returns("3072");

        _handler = new SyncKnowledgeBaseCommandHandler(
            _vectorStore,
            _embeddingService,
            _textChunker,
            _configuration,
            _logger);
    }

    [Fact]
    public async Task Handle_Should_ResolveCollectionNameFromMapping_When_ModelIdConfigured()
    {
        // Arrange
        var modelId = "models/gemini-embedding-2";
        var expectedCollection = "resident_knowledge_base_gemini_2";

        _embeddingService.ModelId.Returns(modelId);
        _configuration[$"Qdrant:Collections:{modelId}"].Returns(expectedCollection);

        var textChunks = new List<TextChunk>
        {
            new TextChunk 
            { 
                ChunkId = "chunk-1", 
                Content = "Nội dung tri thức", 
                ChunkIndex = 0, 
                TokenCount = 10,
                H1 = "Tiêu đề",
                Source = "test.md",
                DocumentType = "Chung"
            }
        };

        _textChunker.SplitText(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(textChunks);

        _embeddingService.GenerateEmbeddingAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new float[3072]));

        var command = new SyncKnowledgeBaseCommand(forceRebuild: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CollectionName.Should().Be(expectedCollection);

        // Verify collection creation & index calls used the correct collection name
        await _vectorStore.Received(1).CreateCollectionIfNotExistsAsync(expectedCollection, 3072, Arg.Any<CancellationToken>());
        await _vectorStore.Received(1).CreatePayloadIndexAsync(expectedCollection, "document_type", "keyword", Arg.Any<CancellationToken>());
        await _vectorStore.Received(1).CreatePayloadIndexAsync(expectedCollection, "source", "keyword", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_FallbackToDefaultCollection_When_NoConfiguredMapping()
    {
        // Arrange
        var modelId = "some-unknown-model";
        _embeddingService.ModelId.Returns(modelId);
        _configuration[$"Qdrant:Collections:{modelId}"].Returns((string?)null);
        _configuration["Qdrant:CollectionName"].Returns((string?)null); // Fallback to resident_knowledge_base

        var textChunks = new List<TextChunk>
        {
            new TextChunk 
            { 
                ChunkId = "chunk-1", 
                Content = "Nội dung tri thức", 
                ChunkIndex = 0, 
                TokenCount = 10,
                H1 = "Tiêu đề",
                Source = "test.md",
                DocumentType = "Chung"
            }
        };

        _textChunker.SplitText(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(textChunks);

        _embeddingService.GenerateEmbeddingAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new float[3072]));

        var command = new SyncKnowledgeBaseCommand(forceRebuild: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CollectionName.Should().Be("resident_knowledge_base");

        await _vectorStore.Received(1).CreateCollectionIfNotExistsAsync("resident_knowledge_base", 3072, Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempKbPath))
        {
            Directory.Delete(_tempKbPath, true);
        }
    }
}
