using FluentAssertions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class ChiSoTieuThuCommandRepositoryTests : BaseIntegrationTest
{
    private readonly ChiSoTieuThuCommandRepository _repository;

    public ChiSoTieuThuCommandRepositoryTests() : base()
    {
        _repository = new ChiSoTieuThuCommandRepository(DbContext);
    }

    private async Task<(CanHo, DichVu)> CreateDependenciesAsync()
    {
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà 01", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ A1-01", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        var dichVu = new DichVu(Guid.NewGuid().ToString()[..10], "Điện Sinh Hoạt", LoaiDichVu.TienIch, "kWh");
        
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.DichVus.AddAsync(dichVu);
        await DbContext.SaveChangesAsync();

        return (canHo, dichVu);
    }

    [Fact]
    public async Task AddAsync_Should_SaveEntityToDatabase()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var chiSo = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);

        // Act
        await _repository.AddAsync(chiSo);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedChiSo = await DbContext.ChiSoTieuThus.FindAsync(chiSo.Id);
        savedChiSo.Should().NotBeNull();
        savedChiSo!.CanHoId.Should().Be(canHo.Id);
        savedChiSo.DichVuId.Should().Be(dichVu.Id);
        savedChiSo.SoLuong.Should().Be(50);
    }

    [Fact]
    public async Task AddRangeAsync_Should_SaveMultipleEntities()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var chiSo1 = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);
        var chiSo2 = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 150, 210, 6, 2024, DateTimeOffset.Now);

        // Act
        await _repository.AddRangeAsync(new[] { chiSo1, chiSo2 });
        await DbContext.SaveChangesAsync();

        // Assert
        var saved1 = await DbContext.ChiSoTieuThus.FindAsync(chiSo1.Id);
        var saved2 = await DbContext.ChiSoTieuThus.FindAsync(chiSo2.Id);
        saved1.Should().NotBeNull();
        saved2.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnEntity_WhenExists()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var chiSo = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);
        await _repository.AddAsync(chiSo);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(chiSo.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(chiSo.Id);
    }

    [Fact]
    public async Task Update_Should_ModifyEntityInDatabase()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var chiSo = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);
        await _repository.AddAsync(chiSo);
        await DbContext.SaveChangesAsync();

        // Act
        chiSo.Update(100, 160, 5, 2024, DateTimeOffset.Now, null, "Updated note");
        _repository.Update(chiSo);
        await DbContext.SaveChangesAsync();

        // Assert
        var updated = await DbContext.ChiSoTieuThus.FindAsync(chiSo.Id);
        updated!.ChiSoMoi.Should().Be(160);
        updated.GhiChu.Should().Be("Updated note");
    }

    [Fact]
    public async Task GetByMaTraCuusAsync_Should_ReturnMatchingEntities()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var chiSo1 = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now, null, null, "MTC1");
        var chiSo2 = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 150, 210, 6, 2024, DateTimeOffset.Now, null, null, "MTC2");
        await _repository.AddRangeAsync(new[] { chiSo1, chiSo2 });
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByMaTraCuusAsync(new[] { "MTC1" });

        // Assert
        result.Should().HaveCount(1);
        result.First().MaTraCuu.Should().Be("MTC1");
    }
}
