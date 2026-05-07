using FluentAssertions;
using NSubstitute;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.XacNhanThanhToanDoiTac;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Application.UnitTests.Abstractions;
using Xunit;

namespace HeThongChungCu.Application.UnitTests.Features.QLDoiTac;

public class HoaDonDoiTacCommandHandlerTests : BaseTest
{
    private readonly IHoaDonDoiTacCommandRepository _hoaDonDoiTacRepo;
    private readonly IDoiTacCommandRepository _doiTacRepo;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepo;
    private readonly IUnitOfWork _unitOfWork;

    public HoaDonDoiTacCommandHandlerTests()
    {
        _hoaDonDoiTacRepo = CreateMock<IHoaDonDoiTacCommandRepository>();
        _doiTacRepo = CreateMock<IDoiTacCommandRepository>();
        _tepTaiLieuRepo = CreateMock<ITepTaiLieuCommandRepository>();
        _unitOfWork = CreateMock<IUnitOfWork>();
    }

    #region Create Command Tests

    [Fact]
    public async Task Create_Should_CreateInvoiceAndMarkFileAsUsed_When_Valid()
    {
        // Arrange
        var hopDong = new HopDongDoiTac(1, "HD001", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 10000000, 1, "Noi dung");
        _doiTacRepo.GetHopDongByIdAsync(1, CancellationToken).Returns(hopDong);

        var doiTac = new DoiTac(tenDoiTac: "Doi tac A", soDienThoai: "0903456789", email: "partner@example.com");
        _doiTacRepo.GetByIdAsync(hopDong.DoiTacId, CancellationToken).Returns(doiTac);

        _hoaDonDoiTacRepo.ExistsByKyAsync(1, 5, 2026, CancellationToken).Returns(false);

        var file = new TepTaiLieu("file.pdf", "https://url.com/file.pdf", "application/pdf");
        _tepTaiLieuRepo.GetByIdAsync(10, CancellationToken).Returns(file);

        var command = new CreateHoaDonDoiTacCommand(1, 5, 2026, 5000000, 10, "Ghi chu hoa don");
        var handler = new CreateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SoTien.Should().Be(5000000);
        result.Value.Thang.Should().Be(5);
        result.Value.Nam.Should().Be(2026);
        result.Value.TrangThaiThanhToanId.Should().Be(TrangThaiThanhToanDoiTac.ChuaThanhToan.Value);

        file.IsUsed.Should().BeTrue();
        _tepTaiLieuRepo.Received(1).Update(file);
        await _hoaDonDoiTacRepo.Received(1).AddAsync(Arg.Any<HoaDonDoiTac>(), CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_ContractNotFound()
    {
        // Arrange
        _doiTacRepo.GetHopDongByIdAsync(1, CancellationToken).Returns((HopDongDoiTac?)null);

        var command = new CreateHoaDonDoiTacCommand(1, 5, 2026, 5000000, null, "Ghi chu");
        var handler = new CreateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(DoiTacErrors.HopDongNotFound);
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_ContractNotActive()
    {
        // Arrange
        var hopDong = new HopDongDoiTac(1, "HD001", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(-1), 10000000, 1, "Noi dung");
        _doiTacRepo.GetHopDongByIdAsync(1, CancellationToken).Returns(hopDong);

        var command = new CreateHoaDonDoiTacCommand(1, 5, 2026, 5000000, null, "Ghi chu");
        var handler = new CreateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(DoiTacErrors.HopDongNotActive);
    }

    [Fact]
    public async Task Create_Should_ReturnFailure_When_DuplicateInvoiceForMonthYear()
    {
        // Arrange
        var hopDong = new HopDongDoiTac(1, "HD001", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 10000000, 1, "Noi dung");
        _doiTacRepo.GetHopDongByIdAsync(1, CancellationToken).Returns(hopDong);

        _hoaDonDoiTacRepo.ExistsByKyAsync(1, 5, 2026, CancellationToken).Returns(true);

        var command = new CreateHoaDonDoiTacCommand(1, 5, 2026, 5000000, null, "Ghi chu");
        var handler = new CreateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(DoiTacErrors.HoaDonDuplicateKy);
    }

    #endregion

    #region Update Command Tests

    [Fact]
    public async Task Update_Should_UpdateInvoiceAndHandleFiles_When_Valid()
    {
        // Arrange
        var oldFile = new TepTaiLieu("old.pdf", "https://url.com/old.pdf", "application/pdf");
        oldFile.MarkAsUsed();

        var hoaDon = new HoaDonDoiTac(1, 5, 2026, 4000000, 10, "old ghi chu");
        hoaDon.GetType().GetProperty("Id")?.SetValue(hoaDon, 99);
        _hoaDonDoiTacRepo.GetByIdAsync(99, CancellationToken).Returns(hoaDon);

        var hopDong = new HopDongDoiTac(1, "HD001", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 10000000, 1, "Noi dung");
        _doiTacRepo.GetHopDongByIdAsync(1, CancellationToken).Returns(hopDong);

        _tepTaiLieuRepo.GetByIdAsync(10, CancellationToken).Returns(oldFile);

        var newFile = new TepTaiLieu("new.pdf", "https://url.com/new.pdf", "application/pdf");
        _tepTaiLieuRepo.GetByIdAsync(20, CancellationToken).Returns(newFile);

        var command = new UpdateHoaDonDoiTacCommand(99, 6, 2026, 4500000, 20, "new ghi chu");
        var handler = new UpdateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Thang.Should().Be(6);
        result.Value.SoTien.Should().Be(4500000);
        result.Value.FileHoaDonId.Should().Be(20);

        oldFile.IsUsed.Should().BeFalse();
        newFile.IsUsed.Should().BeTrue();

        _tepTaiLieuRepo.Received(1).Update(oldFile);
        _tepTaiLieuRepo.Received(1).Update(newFile);
        _hoaDonDoiTacRepo.Received(1).Update(hoaDon);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Update_Should_ReturnFailure_When_InvoiceAlreadyPaid()
    {
        // Arrange
        var hoaDon = new HoaDonDoiTac(1, 5, 2026, 4000000, null, "ghi chu");
        hoaDon.UpdateStatus(TrangThaiThanhToanDoiTac.DaThanhToan);
        _hoaDonDoiTacRepo.GetByIdAsync(99, CancellationToken).Returns(hoaDon);

        var command = new UpdateHoaDonDoiTacCommand(99, 6, 2026, 4500000, null, "new ghi chu");
        var handler = new UpdateHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _doiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(DoiTacErrors.HoaDonAlreadyPaid);
    }

    #endregion

    #region Delete Command Tests

    [Fact]
    public async Task Delete_Should_RemoveInvoiceAndReleaseFile_When_Valid()
    {
        // Arrange
        var file = new TepTaiLieu("file.pdf", "https://url.com/file.pdf", "application/pdf");
        file.MarkAsUsed();

        var hoaDon = new HoaDonDoiTac(1, 5, 2026, 4000000, 10, "ghi chu");
        _hoaDonDoiTacRepo.GetByIdAsync(99, CancellationToken).Returns(hoaDon);
        _tepTaiLieuRepo.GetByIdAsync(10, CancellationToken).Returns(file);

        var command = new DeleteHoaDonDoiTacCommand(99);
        var handler = new DeleteHoaDonDoiTacCommandHandler(_hoaDonDoiTacRepo, _tepTaiLieuRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        file.IsUsed.Should().BeFalse();
        _tepTaiLieuRepo.Received(1).Update(file);
        _hoaDonDoiTacRepo.Received(1).Remove(hoaDon);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    #endregion

    #region Confirm Payment Tests

    [Fact]
    public async Task ConfirmPayment_Should_SetStatusToPaid_When_Valid()
    {
        // Arrange
        var hoaDon = new HoaDonDoiTac(1, 5, 2026, 4000000, null, "ghi chu");
        hoaDon.TrangThaiThanhToanId.Should().Be(TrangThaiThanhToanDoiTac.ChuaThanhToan);
        _hoaDonDoiTacRepo.GetByIdAsync(99, CancellationToken).Returns(hoaDon);

        var command = new XacNhanThanhToanDoiTacCommand(99);
        var handler = new XacNhanThanhToanDoiTacCommandHandler(_hoaDonDoiTacRepo, _unitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        hoaDon.TrangThaiThanhToanId.Should().Be(TrangThaiThanhToanDoiTac.DaThanhToan);

        _hoaDonDoiTacRepo.Received(1).Update(hoaDon);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    #endregion
}
