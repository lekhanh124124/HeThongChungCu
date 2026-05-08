using FluentAssertions;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class ChiSoTieuThuQueryRepositoryTests : BaseIntegrationTest
{
    private readonly ChiSoTieuThuQueryRepository _repository;

    public ChiSoTieuThuQueryRepositoryTests() : base()
    {
        _repository = new ChiSoTieuThuQueryRepository(DbContext);
    }

    private async Task<(ToaNha, Tang, CanHo, DichVu)> CreateDataAsync()
    {
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà 01", "A", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ A1-01", 100, 2, 2, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        await DbContext.CanHos.AddAsync(canHo);

        var dichVu = new DichVu(Guid.NewGuid().ToString()[..10], "Điện Sinh Hoạt", LoaiDichVu.TienIch, "kWh");
        await DbContext.DichVus.AddAsync(dichVu);
        await DbContext.SaveChangesAsync();

        return (toaNha, tang, canHo, dichVu);
    }

    [Fact]
    public async Task GetExcelTemplateDataAsync_Should_ReturnData()
    {
        // Arrange
        var (toaNha, tang, canHo, dichVu) = await CreateDataAsync();
        var spec = new ExportChiSoTemplateSpecification(dichVu.Id, toaNha.Id, tang.Id, 5, 2024);

        // Act
        var result = await _repository.GetExcelTemplateDataAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().MaCanHo.Should().Be(canHo.MaCanHo);
    }

    [Fact]
    public async Task GetListAsync_Should_ReturnPagedData()
    {
        // Arrange
        var (_, _, canHo, dichVu) = await CreateDataAsync();
        var chiSo = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);
        await DbContext.ChiSoTieuThus.AddAsync(chiSo);
        await DbContext.SaveChangesAsync();

        var spec = new GetListChiSoSpecification("Id", false, 1, 10, 5, 2024, dichVu.Id, null, null, null, null);

        // Act
        var result = await _repository.GetListAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items!.First().Id.Should().Be(chiSo.Id);
        result.PagingInfo.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnData()
    {
        // Arrange
        var (_, _, canHo, dichVu) = await CreateDataAsync();
        var chiSo = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 5, 2024, DateTimeOffset.Now);
        await DbContext.ChiSoTieuThus.AddAsync(chiSo);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(new GetChiSoByIdSpecification(chiSo.Id));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(chiSo.Id);
        result.MaCanHo.Should().Be(canHo.MaCanHo);
        result.TenDichVu.Should().Be(dichVu.TenDichVu);
    }
}
