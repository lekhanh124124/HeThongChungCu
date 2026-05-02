using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.ConfirmChiSoBatch;

public sealed class ConfirmChiSoBatchCommandHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ConfirmChiSoBatchCommandHandler _handler;

    public ConfirmChiSoBatchCommandHandlerTests()
    {
        _chiSoRepository = CreateMock<IChiSoTieuThuCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new ConfirmChiSoBatchCommandHandler(_chiSoRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ConfirmAndSaveChanges_When_ValidIds()
    {
        // Arrange
        var chiSo = ChiSoTieuThu.Create(1, 1, 0, 10, 5, 2024, DateTimeOffset.Now);
        _chiSoRepository.GetByIdAsync(1, CancellationToken).Returns(chiSo);
        
        var command = new ConfirmChiSoBatchCommand(new List<int> { 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        _chiSoRepository.Received(1).Update(chiSo);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_Skip_When_IdNotFound()
    {
        // Arrange
        _chiSoRepository.GetByIdAsync(1, CancellationToken).Returns((ChiSoTieuThu?)null);
        var command = new ConfirmChiSoBatchCommand(new List<int> { 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
        _chiSoRepository.DidNotReceive().Update(Arg.Any<ChiSoTieuThu>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_Skip_When_ConfirmFails()
    {
        // Arrange
        var chiSo = ChiSoTieuThu.Create(1, 1, 0, 10, 5, 2024, DateTimeOffset.Now);
        chiSo.Confirm(); 
        chiSo.MarkAsBilled(1); // Locked state
        
        _chiSoRepository.GetByIdAsync(1, CancellationToken).Returns(chiSo);
        var command = new ConfirmChiSoBatchCommand(new List<int> { 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0); 
        _chiSoRepository.DidNotReceive().Update(chiSo);
    }
}
