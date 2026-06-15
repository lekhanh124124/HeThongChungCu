using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLChiSoTieuThu.ConfirmChiSoBatch;

public sealed class ConfirmChiSoBatchCommandHandlerTests : BaseTest
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IDichVuCommandRepository _dichVuRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ConfirmChiSoBatchCommandHandler _handler;

    public ConfirmChiSoBatchCommandHandlerTests()
    {
        _chiSoRepository = CreateMock<IChiSoTieuThuCommandRepository>();
        _dichVuRepository = CreateMock<IDichVuCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();

        _handler = new ConfirmChiSoBatchCommandHandler(_chiSoRepository, _dichVuRepository, _unitOfWork);
    }

    private DichVu CreateActiveDichVu()
    {
        var dichVu = new DichVu("DV001", "Điện", LoaiDichVu.TienIch, "kWh");
        dichVu.Activate();
        dichVu.AddBangGiaLuyTien("Bảng giá điện", DateTimeOffset.Now.AddDays(-1), true);
        dichVu.BangGias.First().Activate();
        return dichVu;
    }

    [Fact]
    public async Task Handle_Should_ConfirmAndSaveChanges_When_ValidIds()
    {
        // Arrange
        var chiSo = ChiSoTieuThu.Create(1, 1, 0, 10, 5, 2024, DateTimeOffset.Now);
        var dichVu = CreateActiveDichVu();

        _dichVuRepository.GetByIdWithBangGiasAsync(1, CancellationToken).Returns(dichVu);
        _chiSoRepository.GetDraftByPeriodAsync(5, 2024, 1, CancellationToken).Returns(new List<ChiSoTieuThu> { chiSo });
        
        var command = new ConfirmChiSoBatchCommand(5, 2024, 1);

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
        var dichVu = CreateActiveDichVu();

        _dichVuRepository.GetByIdWithBangGiasAsync(1, CancellationToken).Returns(dichVu);
        _chiSoRepository.GetDraftByPeriodAsync(5, 2024, 1, CancellationToken).Returns(new List<ChiSoTieuThu>());
        
        var command = new ConfirmChiSoBatchCommand(5, 2024, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors[0].Code.Should().Be("Confirm.NoDraft");
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
        
        var dichVu = CreateActiveDichVu();

        _dichVuRepository.GetByIdWithBangGiasAsync(1, CancellationToken).Returns(dichVu);
        _chiSoRepository.GetDraftByPeriodAsync(5, 2024, 1, CancellationToken).Returns(new List<ChiSoTieuThu> { chiSo });
        
        var command = new ConfirmChiSoBatchCommand(5, 2024, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0); 
        _chiSoRepository.DidNotReceive().Update(chiSo);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken);
    }
}
