using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.SyncTriThucChatbot;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLTriThucChatbot;

public class SyncTriThucChatbotCommandHandlerTests
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly ITextChunker _textChunker;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SyncTriThucChatbotCommandHandler> _logger;
    private readonly SyncTriThucChatbotCommandHandler _handler;

    public SyncTriThucChatbotCommandHandlerTests()
    {
        _repository = Substitute.For<ITriThucChatbotCommandRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _embeddingService = Substitute.For<IEmbeddingService>();
        _vectorStore = Substitute.For<IVectorStore>();
        _textChunker = Substitute.For<ITextChunker>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<SyncTriThucChatbotCommandHandler>>();

        _configuration["AI:Provider"].Returns("OpenAI");
        _configuration["Qdrant:CollectionName"].Returns("resident_knowledge_base");
        _embeddingService.ModelId.Returns("text-embedding-3-small");

        _handler = new SyncTriThucChatbotCommandHandler(
            _repository,
            _unitOfWork,
            _embeddingService,
            _vectorStore,
            _textChunker,
            _configuration,
            _logger
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithEmpty_When_NoRecordsToSync()
    {
        // Arrange
        _repository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<TriThucChatbot>()));
        _repository.GetSyncedInactiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<TriThucChatbot>()));

        var command = new SyncTriThucChatbotCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UpsertedCount.Should().Be(0);
        result.Value.DeletedCount.Should().Be(0);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_SyncActiveRecords_ByChunkingAndEmbedding()
    {
        // Arrange
        var t1 = TriThucChatbot.CreateTriThucChatbot("Nuôi chó dữ", "Quy định cấm nuôi chó dữ", "faq", 1).Value;
        // set ID via Reflection or use it as is (Id defaults to 0 but we can pretend)
        
        var activeList = new List<TriThucChatbot> { t1 };
        _repository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(activeList));
        _repository.GetSyncedInactiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<TriThucChatbot>()));

        var chunks = new List<TextChunk>
        {
            new() { ChunkId = "c1", Content = "Quy định cấm nuôi chó dữ", ChunkIndex = 0, TokenCount = 10, H1 = "Nuôi chó dữ" }
        };
        _textChunker.SplitText(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(chunks);

        var vector = new float[1536];
        _embeddingService.GenerateEmbeddingAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(vector));

        var command = new SyncTriThucChatbotCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UpsertedCount.Should().Be(1);
        result.Value.DeletedCount.Should().Be(0);

        t1.IsSynced.Should().BeTrue();
        t1.LastSyncedAt.Should().NotBeNull();

        await _vectorStore.Received(1).DeleteBySourceAsync(Arg.Any<string>(), "db:tri-thuc-chatbot:0", Arg.Any<CancellationToken>());
        await _vectorStore.Received(1).UpsertVectorsBatchAsync(Arg.Any<string>(), Arg.Any<List<VectorRecord>>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_DeleteSyncedInactiveRecords_FromQdrant()
    {
        // Arrange
        var t1 = TriThucChatbot.CreateTriThucChatbot("Tài liệu cũ", "Nội dung cũ", "faq", 1).Value;
        t1.MarkAsSynced(DateTimeOffset.UtcNow); // đã từng sync
        t1.Deactivate(); // nay đã inactive

        var inactiveList = new List<TriThucChatbot> { t1 };
        _repository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<TriThucChatbot>()));
        _repository.GetSyncedInactiveAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(inactiveList));

        var command = new SyncTriThucChatbotCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UpsertedCount.Should().Be(0);
        result.Value.DeletedCount.Should().Be(1);

        t1.IsSynced.Should().BeFalse();
        t1.LastSyncedAt.Should().BeNull();

        await _vectorStore.Received(1).DeleteBySourceAsync(Arg.Any<string>(), "db:tri-thuc-chatbot:0", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
