using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ToggleActiveTriThucChatbot;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLTriThucChatbot;

public class ToggleActiveTriThucChatbotCommandHandlerTests
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ToggleActiveTriThucChatbotCommandHandler _handler;

    public ToggleActiveTriThucChatbotCommandHandlerTests()
    {
        _repository = Substitute.For<ITriThucChatbotCommandRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new ToggleActiveTriThucChatbotCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessAndActivate_When_ItemExistsAndActivateIsTrue()
    {
        // Arrange
        var triThuc = TriThucChatbot.CreateTriThucChatbot("Tiêu đề", "Nội dung", "faq").Value;
        triThuc.Deactivate(); // Bắt đầu ở trạng thái false
        
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(triThuc);

        var command = new ToggleActiveTriThucChatbotCommand(Id: 1, Activate: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        triThuc.IsActive.Should().BeTrue();
        triThuc.IsSynced.Should().BeFalse(); // Thay đổi trạng thái hoạt động reset cờ sync

        _repository.Received(1).Update(triThuc);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessAndDeactivate_When_ItemExistsAndActivateIsFalse()
    {
        // Arrange
        var triThuc = TriThucChatbot.CreateTriThucChatbot("Tiêu đề", "Nội dung", "faq").Value;
        triThuc.Activate(); // Bắt đầu ở trạng thái true
        
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(triThuc);

        var command = new ToggleActiveTriThucChatbotCommand(Id: 1, Activate: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        triThuc.IsActive.Should().BeFalse();
        triThuc.IsSynced.Should().BeFalse(); // Thay đổi trạng thái hoạt động reset cờ sync

        _repository.Received(1).Update(triThuc);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureNotFound_When_ItemDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>())
            .Returns((TriThucChatbot?)null);

        var command = new ToggleActiveTriThucChatbotCommand(Id: 99, Activate: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.NotFound");
        _repository.DidNotReceive().Update(Arg.Any<TriThucChatbot>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
