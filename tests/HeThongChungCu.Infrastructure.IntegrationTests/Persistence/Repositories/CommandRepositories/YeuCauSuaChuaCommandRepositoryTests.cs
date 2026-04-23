using FluentAssertions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories.CommandRepositories;

public class YeuCauSuaChuaCommandRepositoryTests : BaseIntegrationTest
{
    private readonly YeuCauSuaChuaCommandRepository _repository;

    public YeuCauSuaChuaCommandRepositoryTests()
    {
        _repository = new YeuCauSuaChuaCommandRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldAddYeuCauSuaChua()
    {
        // Arrange
        var canHoId = await SeedCanHoAsync();
        var ycsc = YeuCauSuaChua.Create(
            canHoId,
            PhamViSuaChua.TrongCanHo,
            LoaiSuCoKyThuat.Dien,
            MucDoUuTien.Thuong,
            "Hỏng bóng đèn",
            "Phòng khách");

        // Act
        await _repository.AddAsync(ycsc);
        await DbContext.SaveChangesAsync();

        // Assert
        var result = await DbContext.YeuCauSuaChuas.FindAsync(ycsc.Id);
        result.Should().NotBeNull();
        result!.NoiDung.Should().Be("Hỏng bóng đèn");
    }

    [Fact]
    public async Task GetByIdWithFilesAsync_ShouldReturnYcscWithFiles()
    {
        // Arrange
        var canHoId = await SeedCanHoAsync();
        var tep = new TepYeuCauSuaChua("test.jpg", "url", 1024, "image/jpeg");
        var ycsc = YeuCauSuaChua.Create(
            canHoId,
            PhamViSuaChua.TrongCanHo,
            LoaiSuCoKyThuat.Dien,
            MucDoUuTien.Thuong,
            "Hỏng bóng đèn",
            "Phòng khách",
            new List<TepYeuCauSuaChua> { tep });

        await DbContext.YeuCauSuaChuas.AddAsync(ycsc);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithFilesAsync(ycsc.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TepYeuCauSuaChuas.Should().HaveCount(1);
        result.TepYeuCauSuaChuas.First().FileName.Should().Be("test.jpg");
    }

    [Fact]
    public async Task GetByIdWithPersonnelAsync_ShouldReturnYcscWithPersonnel()
    {
        // Arrange
        var canHoId = await SeedCanHoAsync();
        var nhanVienId = await SeedNhanVienAsync();
        var ycsc = YeuCauSuaChua.Create(
            canHoId,
            PhamViSuaChua.TrongCanHo,
            LoaiSuCoKyThuat.Dien,
            MucDoUuTien.Thuong,
            "Hỏng bóng đèn",
            "Phòng khách");

        ycsc.AssignInternalStaff(nhanVienId);

        await DbContext.YeuCauSuaChuas.AddAsync(ycsc);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithPersonnelAsync(ycsc.Id);

        // Assert
        result.Should().NotBeNull();
        result!.NhanSuSuaChuas.Should().HaveCount(1);
        result.NhanSuSuaChuas.First().NhanVienId.Should().Be(nhanVienId);
    }

    [Fact]
    public async Task Update_ShouldUpdateEntity()
    {
        // Arrange
        var canHoId = await SeedCanHoAsync();
        var nhanVienId = await SeedNhanVienAsync();
        var ycsc = YeuCauSuaChua.Create(
            canHoId,
            PhamViSuaChua.TrongCanHo,
            LoaiSuCoKyThuat.Dien,
            MucDoUuTien.Thuong,
            "Hỏng bóng đèn",
            "Phòng khách");

        await DbContext.YeuCauSuaChuas.AddAsync(ycsc);
        await DbContext.SaveChangesAsync();

        // Act
        ycsc.TiepNhan(nhanVienId, DateTimeOffset.Now);
        _repository.Update(ycsc);
        await DbContext.SaveChangesAsync();

        // Assert
        var result = await DbContext.YeuCauSuaChuas.FindAsync(ycsc.Id);
        result!.TrangThaiSuaChuaId.Should().Be(TrangThaiSuaChua.DaTiepNhan);
        result.NguoiXuLyId.Should().Be(nhanVienId);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntity()
    {
        // Arrange
        var canHoId = await SeedCanHoAsync();
        var ycsc = YeuCauSuaChua.Create(
            canHoId,
            PhamViSuaChua.TrongCanHo,
            LoaiSuCoKyThuat.Dien,
            MucDoUuTien.Thuong,
            "Hỏng bóng đèn",
            "Phòng khách");

        await DbContext.YeuCauSuaChuas.AddAsync(ycsc);
        await DbContext.SaveChangesAsync();

        // Act
        _repository.Delete(ycsc);
        await DbContext.SaveChangesAsync();

        // Assert
        // Use a clean context to verify Global Query Filter (Soft Delete)
        using var verifyContext = CreateDbContext();
        
        // Check with IgnoreQueryFilters to see what happened to the entity
        var softDeletedResult = await verifyContext.YeuCauSuaChuas
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == ycsc.Id);
        
        softDeletedResult.Should().NotBeNull("Entity should still exist in DB");
        softDeletedResult!.IsDeleted.Should().BeTrue("Entity should have IsDeleted = true");

        // Now check if it's filtered out
        var result = await verifyContext.YeuCaus
            .FirstOrDefaultAsync(x => x.Id == ycsc.Id);
        result.Should().BeNull("Entity should be filtered out by Global Query Filter");
    }

    private async Task<int> SeedCanHoAsync()
    {
        var toaNha = new ToaNha("T1", "Toà T1", "A", "Dia Chi", "Mo Ta", TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang("T1-1", "Tầng 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(
            tang.Id,
            "C101",
            "Căn hộ 101",
            70.5m,
            2,
            1,
            LoaiCanHo.Standard,
            TrangThaiCanHo.DangTrong);
        
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        return canHo.Id;
    }

    private async Task<int> SeedNhanVienAsync()
    {
        var nguoiDung = new NguoiDung("Teo", "Nguyen Van", DateTimeOffset.Now.AddYears(-20), GioiTinh.Nam, "Dia Chi");
        await DbContext.NguoiDung.AddAsync(nguoiDung);
        await DbContext.SaveChangesAsync();

        var nhanVien = NhanVien.CreateNhanVien(nguoiDung.Id, LoaiNhanVien.KyThuat, "NV001", DateTimeOffset.Now);
        await DbContext.NhanViens.AddAsync(nhanVien);
        await DbContext.SaveChangesAsync();

        return nhanVien.Id;
    }
}
