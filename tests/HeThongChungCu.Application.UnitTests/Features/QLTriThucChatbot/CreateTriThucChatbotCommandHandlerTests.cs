using FluentAssertions;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.CreateTriThucChatbot;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLTriThucChatbot;

public class CreateTriThucChatbotCommandHandlerTests
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateTriThucChatbotCommandHandler _handler;

    public CreateTriThucChatbotCommandHandlerTests()
    {
        _repository = Substitute.For<ITriThucChatbotCommandRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateTriThucChatbotCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InputIsValid()
    {
        // Arrange
        var command = new CreateTriThucChatbotCommand(
            TieuDe: "Quy định nuôi thú cưng",
            NoiDung: "Cấm nuôi chó dữ tại chung cư",
            DanhMuc: "faq",
            ThuTuHienThi: 1
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TieuDe.Should().Be("Quy định nuôi thú cưng");
        result.Value.NoiDung.Should().Be("Cấm nuôi chó dữ tại chung cư");
        result.Value.DanhMuc.Should().Be("faq");
        result.Value.ThuTuHienThi.Should().Be(1);
        result.Value.IsActive.Should().BeFalse(); // Mặc định tạo mới là deactive

        _repository.Received(1).Add(Arg.Any<TriThucChatbot>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TieuDeIsEmpty()
    {
        // Arrange
        var command = new CreateTriThucChatbotCommand(
            TieuDe: "",
            NoiDung: "Nội dung",
            DanhMuc: "faq"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.TieuDeRequired");
        _repository.DidNotReceive().Add(Arg.Any<TriThucChatbot>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoiDungIsEmpty()
    {
        // Arrange
        var command = new CreateTriThucChatbotCommand(
            TieuDe: "Tiêu đề",
            NoiDung: "  ",
            DanhMuc: "faq"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.NoiDungRequired");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DanhMucIsEmpty()
    {
        // Arrange
        var command = new CreateTriThucChatbotCommand(
            TieuDe: "Tiêu đề",
            NoiDung: "Nội dung",
            DanhMuc: null!
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.First().Code.Should().Be("TriThucChatbot.DanhMucRequired");
    }
}
