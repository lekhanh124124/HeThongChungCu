using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.DeleteTriThucChatbot;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLTriThucChatbot;

public class DeleteTriThucChatbotCommandHandlerTests
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteTriThucChatbotCommandHandler _handler;

    public DeleteTriThucChatbotCommandHandlerTests()
    {
        _repository = Substitute.For<ITriThucChatbotCommandRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new DeleteTriThucChatbotCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AllItemsAreInactive()
    {
        // Arrange
        var t1 = TriThucChatbot.CreateTriThucChatbot("Tiêu đề 1", "Nội dung 1", "faq").Value;
        var t2 = TriThucChatbot.CreateTriThucChatbot("Tiêu đề 2", "Nội dung 2", "faq").Value;
        
        t1.Deactivate(); // Cả hai đều deactive
        t2.Deactivate();

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(t1);
        _repository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(t2);

        var command = new DeleteTriThucChatbotCommand(new List<int> { 1, 2 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        _repository.Received(1).Remove(t1);
        _repository.Received(1).Remove(t2);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureCannotDeleteActive_When_AnyItemIsActive()
    {
        // Arrange
        var t1 = TriThucChatbot.CreateTriThucChatbot("Tiêu đề 1", "Nội dung 1", "faq").Value;
        var t2 = TriThucChatbot.CreateTriThucChatbot("Tiêu đề 2", "Nội dung 2", "faq").Value;
        
        t1.Deactivate();
        t2.Activate(); // t2 đang active!

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(t1);
        _repository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(t2);

        var command = new DeleteTriThucChatbotCommand(new List<int> { 1, 2 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.CannotDeleteActive");

        _repository.DidNotReceive().Remove(Arg.Any<TriThucChatbot>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureDeleteEmpty_When_IdsListIsEmpty()
    {
        // Arrange
        var command = new DeleteTriThucChatbotCommand(new List<int>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.DeleteEmpty");
    }
}
