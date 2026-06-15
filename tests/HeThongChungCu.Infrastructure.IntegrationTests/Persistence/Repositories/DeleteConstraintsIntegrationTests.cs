using FluentAssertions;
using HeThongChungCu.Application.Features.CanHo.Commands.DeleteCanHo;
using HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;
using HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteDoiTac;
using HeThongChungCu.Application.Features.QLNhanVien.Commands.DeleteNhanVien;
using HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;
using HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class DeleteConstraintsIntegrationTests : BaseIntegrationTest
{
    private readonly ToaNhaCommandRepository _toaNhaRepo;
    private readonly CanHoCommandRepository _canHoRepo;
    private readonly QuanHeCuTruCommandRepository _quanHeCuTruRepo;
    private readonly PhuongTienCommandRepository _phuongTienRepo;
    private readonly HoaDonCommandRepository _hoaDonRepo;
    private readonly DichVuCommandRepository _dichVuRepo;
    private readonly DangKyDichVuCommandRepository _dangKyDichVuRepo;
    private readonly NhanVienCommandRepository _nhanVienRepo;

    public DeleteConstraintsIntegrationTests() : base()
    {
        _toaNhaRepo = new ToaNhaCommandRepository(DbContext);
        _canHoRepo = new CanHoCommandRepository(DbContext);
        _quanHeCuTruRepo = new QuanHeCuTruCommandRepository(DbContext);
        _phuongTienRepo = new PhuongTienCommandRepository(DbContext);
        _hoaDonRepo = new HoaDonCommandRepository(DbContext);
        _dichVuRepo = new DichVuCommandRepository(DbContext);
        _dangKyDichVuRepo = new DangKyDichVuCommandRepository(DbContext);
        _nhanVienRepo = new NhanVienCommandRepository(DbContext);
    }

    [Fact]
    public async Task DeleteToaNha_ShouldFail_WhenHasTangs()
    {
        // Arrange
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var handler = new DeleteToaNhaCommandHandler(_toaNhaRepo);

        // Act
        var result = await handler.Handle(new DeleteToaNhaCommand(new[] { toaNha.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "ToaNha.HasTangs");
    }

    [Fact]
    public async Task DeleteToaNha_ShouldSucceed_WhenAllTangsAreDeleted()
    {
        // Arrange
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        // Xóa mềm Tầng
        DbContext.Tangs.Remove(tang);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear(); // Clear tracking so query filters work fresh

        var tangFromDb = await DbContext.Tangs.IgnoreQueryFilters().FirstAsync(t => t.Id == tang.Id);
        tangFromDb.IsDeleted.Should().BeTrue("Bởi vì tầng đã bị xóa mềm");

        var tangCount = await DbContext.Tangs.CountAsync();
        tangCount.Should().Be(0, "Query filter phải lọc các record IsDeleted = true");

        DbContext.ChangeTracker.Clear();

        var toaNhaRepo = new ToaNhaCommandRepository(DbContext);
        var handler = new DeleteToaNhaCommandHandler(toaNhaRepo);

        // Act
        var result = await handler.Handle(new DeleteToaNhaCommand(new[] { toaNha.Id }), CancellationToken.None);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTang_ShouldFail_WhenHasCanHos()
    {
        // Arrange
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "CH1", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        var handler = new DeleteTangCommandHandler(_toaNhaRepo, _canHoRepo);

        // Act
        var result = await handler.Handle(new DeleteTangCommand(new[] { tang.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Tang.HasCanHo");
    }

    [Fact]
    public async Task DeleteCanHo_ShouldFail_WhenHasResidents()
    {
        // Arrange
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "CH1", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        
        var user = new NguoiDung("Van A", "Nguyen", DateTimeOffset.Now.AddYears(-20), GioiTinh.Nam, "HCM");
        await DbContext.NguoiDung.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTimeOffset.Now);
        await DbContext.QuanHeCuTrus.AddAsync(qh);
        await DbContext.SaveChangesAsync();

        var handler = new DeleteCanHoCommandHandler(_canHoRepo, DbContext, _quanHeCuTruRepo, _toaNhaRepo, _phuongTienRepo, _hoaDonRepo);

        // Act
        var result = await handler.Handle(new DeleteCanHoCommand(new[] { canHo.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "CanHo.HasResidencyHistory");
    }

    [Fact]
    public async Task DeleteDichVu_ShouldFail_WhenHasRegistrations()
    {
        // Arrange
        var dichVu = new DichVu(Guid.NewGuid().ToString()[..10], "Dich Vu 1", LoaiDichVu.TienIch, "VND");
        await DbContext.DichVus.AddAsync(dichVu);
        
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "CH1", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        var dangKy = new DangKyDichVu(canHo.Id, dichVu.Id, DateTimeOffset.Now);
        await DbContext.DangKyDichVus.AddAsync(dangKy);
        await DbContext.SaveChangesAsync();

        var handler = new DeleteDichVuCommandHandler(_dichVuRepo, _dangKyDichVuRepo, DbContext);

        // Act
        var result = await handler.Handle(new DeleteDichVuCommand(new List<int> { dichVu.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "DichVu.HasRegistrations");
    }

    [Fact]
    public async Task DeleteNhanVien_ShouldFail_WhenHasAssignedRequests()
    {
        // Arrange
        var user = new NguoiDung("Van B", "Nguyen", DateTimeOffset.Now.AddYears(-20), GioiTinh.Nam, "HCM");
        await DbContext.NguoiDung.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var nhanVien = new NhanVien(user.Id, LoaiNhanVien.KyThuat, "NV001", DateTimeOffset.Now);
        await DbContext.NhanViens.AddAsync(nhanVien);
        await DbContext.SaveChangesAsync();

        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "CH1", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        var yeuCau = YeuCauSuaChua.Create(canHo.Id, PhamViSuaChua.TrongCanHo, LoaiSuCoKyThuat.Dien, "Bong den bi hu", null, null, TrangThaiYeuCau.Approved);
        await DbContext.YeuCauSuaChuas.AddAsync(yeuCau);
        await DbContext.SaveChangesAsync();

        yeuCau.AssignInternalStaff(new[] { nhanVien.Id });
        await DbContext.SaveChangesAsync();

        var dateTimeProvider = NSubstitute.Substitute.For<HeThongChungCu.Application.Common.Interfaces.Services.IDateTimeProvider>();
        dateTimeProvider.Now.Returns(DateTimeOffset.Now);

        var handler = new DeleteNhanVienCommandHandler(_nhanVienRepo, dateTimeProvider, DbContext);

        // Act
        var result = await handler.Handle(new DeleteNhanVienCommand(new List<int> { nhanVien.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "NhanVien.HasAssignedRequests");
    }

    [Fact]
    public async Task DeleteDoiTacs_ShouldFail_WhenHasHopDongs()
    {
        // Arrange
        var doiTac = new DoiTac("Cong ty A");
        await DbContext.DoiTacs.AddAsync(doiTac);
        await DbContext.SaveChangesAsync();

        var dichVu = new DichVu("DV02", "Dich Vu 2", LoaiDichVu.VanHanh, "Thang", "Mo ta");
        await DbContext.DichVus.AddAsync(dichVu);
        await DbContext.SaveChangesAsync();

        doiTac.KyHopDongMoi("HD01", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 1000000, dichVu.Id, null);
        await DbContext.SaveChangesAsync();

        var doiTacRepo = new DoiTacCommandRepository(DbContext);
        var handler = new DeleteDoiTacsCommandHandler(doiTacRepo, DbContext);

        // Act
        var result = await handler.Handle(new DeleteDoiTacsCommand(new List<int> { doiTac.Id }), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "DoiTac.HasHopDongs");
    }

    [Fact]
    public async Task KetThucCuTru_ShouldFail_WhenHasUnpaidInvoices()
    {
        // Arrange
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "TA1", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1", "Tang 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "CH1", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);
        
        var user = new NguoiDung("Van A", "Nguyen", DateTimeOffset.Now.AddYears(-20), GioiTinh.Nam, "HCM");
        await DbContext.NguoiDung.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var quanHe = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTimeOffset.Now);
        await DbContext.QuanHeCuTrus.AddAsync(quanHe);
        await DbContext.SaveChangesAsync();

        var hoaDon = HoaDon.CreateHoaDon(canHo.Id, null, "HD01", new HeThongChungCu.Domain.ValueObjects.KyThanhToan(1, 2026), DateTimeOffset.Now, DateTimeOffset.Now.AddDays(15)).Value;
        
        // Use reflection to set TrangThaiHoaDonId
        var propertyInfo = typeof(HoaDon).GetProperty("TrangThaiHoaDonId");
        propertyInfo?.SetValue(hoaDon, TrangThaiHoaDon.ChuaThanhToan);

        await DbContext.HoaDons.AddAsync(hoaDon);
        await DbContext.SaveChangesAsync();

        var userRepository = new NguoiDungCommandRepository(DbContext);
        var dateTimeProvider = NSubstitute.Substitute.For<HeThongChungCu.Application.Common.Interfaces.Services.IDateTimeProvider>();
        dateTimeProvider.Now.Returns(DateTimeOffset.Now);

        var handler = new KetThucCuTruCommandHandler(_quanHeCuTruRepo, userRepository, _canHoRepo, _toaNhaRepo, _hoaDonRepo, DbContext, dateTimeProvider);

        // Act
        var result = await handler.Handle(new KetThucCuTruCommand(quanHe.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "KetThucCuTru.HasUnpaidInvoices");
    }
}
