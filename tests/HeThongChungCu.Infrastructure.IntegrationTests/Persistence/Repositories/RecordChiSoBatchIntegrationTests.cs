using FluentAssertions;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class RecordChiSoBatchIntegrationTests : BaseIntegrationTest
{
    private readonly ChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly CanHoCommandRepository _canHoRepository;
    private readonly DichVuCommandRepository _dichVuRepository;
    private readonly RecordChiSoBatchCommandHandler _handler;

    public RecordChiSoBatchIntegrationTests() : base()
    {
        _chiSoRepository = new ChiSoTieuThuCommandRepository(DbContext);
        _canHoRepository = new CanHoCommandRepository(DbContext);
        _dichVuRepository = new DichVuCommandRepository(DbContext);
        _handler = new RecordChiSoBatchCommandHandler(
            _chiSoRepository,
            _canHoRepository,
            _dichVuRepository,
            DbContext);
    }

    private async Task<(CanHo CanHo, DichVu DichVu)> CreateDependenciesAsync()
    {
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà Test", "T", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng Test", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ T1-01", 80, 2, 1, LoaiCanHo.Studio, TrangThaiCanHo.ChuaBanGiao);
        var dichVu = new DichVu(Guid.NewGuid().ToString()[..10], "Điện Test", LoaiDichVu.TienIch, "kWh");

        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.DichVus.AddAsync(dichVu);
        await DbContext.SaveChangesAsync();

        return (canHo, dichVu);
    }

    [Fact]
    public async Task Handle_Should_RecordIndicesSuccessfully_And_GenerateCorrectMaTraCuu()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var itemDto = new ChiSoBatchItemDto
        {
            CanHoId = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            DichVuId = dichVu.Id,
            TenDichVu = dichVu.TenDichVu,
            ChiSoCu = 100,
            ChiSoMoi = 150,
            GhiChu = "Ghi chú test"
        };

        var command = new RecordChiSoBatchCommand(
            Items: new List<ChiSoBatchItemDto> { itemDto },
            Thang: 6,
            Nam: 2026,
            NgayGhiNhan: DateTimeOffset.UtcNow
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(1);
        result.Value.FailedCount.Should().Be(0);
        result.Value.Errors.Should().BeEmpty();

        // Verify database state
        var savedList = await _chiSoRepository.GetByPeriodAsync(6, 2026, CancellationToken.None);
        savedList.Should().HaveCount(1);

        var savedChiSo = savedList.First();
        savedChiSo.CanHoId.Should().Be(canHo.Id);
        savedChiSo.DichVuId.Should().Be(dichVu.Id);
        savedChiSo.ChiSoCu.Should().Be(100);
        savedChiSo.ChiSoMoi.Should().Be(150);
        savedChiSo.SoLuong.Should().Be(50);
        savedChiSo.GhiChu.Should().Be("Ghi chú test");
        
        // MaTraCuu must be correctly generated as {MaCanHo}_{DichVuId}_{Thang}_{Nam}
        var expectedMaTraCuu = $"{canHo.MaCanHo}_{dichVu.Id}_6_2026";
        savedChiSo.MaTraCuu.Should().Be(expectedMaTraCuu);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_When_IndexAlreadyExistsInDb()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();

        // Seed an existing reading in database
        var existingReading = ChiSoTieuThu.Create(canHo.Id, dichVu.Id, 100, 150, 6, 2026, DateTimeOffset.UtcNow);
        await _chiSoRepository.AddAsync(existingReading);
        await DbContext.SaveChangesAsync();

        var itemDto = new ChiSoBatchItemDto
        {
            CanHoId = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            DichVuId = dichVu.Id,
            TenDichVu = dichVu.TenDichVu,
            ChiSoCu = 150,
            ChiSoMoi = 200
        };

        var command = new RecordChiSoBatchCommand(
            Items: new List<ChiSoBatchItemDto> { itemDto },
            Thang: 6,
            Nam: 2026,
            NgayGhiNhan: DateTimeOffset.UtcNow
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(0);
        result.Value.FailedCount.Should().Be(1);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors.First().Reason.Should().Be("Đã tồn tại chỉ số cho kỳ này trong hệ thống.");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_When_DuplicateItemsInRequest()
    {
        // Arrange
        var (canHo, dichVu) = await CreateDependenciesAsync();
        var item1 = new ChiSoBatchItemDto
        {
            CanHoId = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            DichVuId = dichVu.Id,
            TenDichVu = dichVu.TenDichVu,
            ChiSoCu = 100,
            ChiSoMoi = 150
        };
        var item2 = new ChiSoBatchItemDto
        {
            CanHoId = canHo.Id,
            MaCanHo = canHo.MaCanHo,
            DichVuId = dichVu.Id,
            TenDichVu = dichVu.TenDichVu,
            ChiSoCu = 150,
            ChiSoMoi = 200
        };

        var command = new RecordChiSoBatchCommand(
            Items: new List<ChiSoBatchItemDto> { item1, item2 },
            Thang: 6,
            Nam: 2026,
            NgayGhiNhan: DateTimeOffset.UtcNow
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(1);
        result.Value.FailedCount.Should().Be(1);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors.First().Reason.Should().Be("Dữ liệu bị trùng lặp trong danh sách gửi lên.");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_When_CanHoOrDichVuDoesNotExist()
    {
        // Arrange
        var itemWithInvalidCanHo = new ChiSoBatchItemDto
        {
            CanHoId = 999999, // Invalid CanHoId
            DichVuId = 1,
            ChiSoCu = 100,
            ChiSoMoi = 150
        };

        var command = new RecordChiSoBatchCommand(
            Items: new List<ChiSoBatchItemDto> { itemWithInvalidCanHo },
            Thang: 6,
            Nam: 2026,
            NgayGhiNhan: DateTimeOffset.UtcNow
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SuccessCount.Should().Be(0);
        result.Value.FailedCount.Should().Be(1);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors.First().Reason.Should().Be("Căn hộ không tồn tại trong hệ thống.");
    }
}
