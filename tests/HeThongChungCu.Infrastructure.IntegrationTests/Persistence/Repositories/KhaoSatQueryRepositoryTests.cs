using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;

public class KhaoSatQueryRepositoryTests : BaseIntegrationTest
{
    private readonly KhaoSatQueryRepository _repository;

    public KhaoSatQueryRepositoryTests() : base()
    {
        _repository = new KhaoSatQueryRepository(DbContext);
    }

    private async Task<(KhaoSat Campaign, CanHo CanHo, NguoiDung Resident)> CreateDataAsync()
    {
        // 1. Create building & floor & apartment
        var toaNha = new ToaNha(Guid.NewGuid().ToString()[..10], "Tòa nhà Khảo sát", "K", null, null, TrangThaiToaNha.DangHoatDong);
        await DbContext.ToaNhas.AddAsync(toaNha);
        await DbContext.SaveChangesAsync();

        var tang = toaNha.AddTang(Guid.NewGuid().ToString()[..10], "Tầng 1", LoaiTang.TangLau);
        await DbContext.SaveChangesAsync();

        var canHo = CanHo.Create(tang.Id, Guid.NewGuid().ToString()[..10], "Căn Hộ K1-01", 120.5m, 3, 2, LoaiCanHo.Standard, TrangThaiCanHo.CoCuDan);
        await DbContext.CanHos.AddAsync(canHo);
        await DbContext.SaveChangesAsync();

        // 2. Create Resident User
        var resident = NguoiDung.CreateNguoiDung(
            "An", "Nguyễn", DateTimeOffset.Now.AddYears(-30), GioiTinh.Nam, "Căn Hộ K1-01, Block K", "123456789012"
        );
        await DbContext.NguoiDung.AddAsync(resident);
        await DbContext.SaveChangesAsync();

        // 3. Create QuanHeCuTru
        var quanHe = new QuanHeCuTru(canHo.Id, resident.Id, LoaiQuanHeCuTru.ChuHo, DateTimeOffset.Now);
        await DbContext.QuanHeCuTrus.AddAsync(quanHe);
        await DbContext.SaveChangesAsync();

        // 4. Create Survey Campaign
        var campaignResult = KhaoSat.Create(
            "Khảo sát chất lượng dịch vụ gửi xe",
            "Đánh giá về nhà xe thông minh",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddDays(5)
        );
        campaignResult.IsSuccess.Should().BeTrue();
        var campaign = campaignResult.Value;

        // Add a dummy question to be able to publish
        var questionResult = campaign.ThemCauHoi("Dịch vụ gửi xe tốt không?", true, false, new List<string> { "Tốt", "Bình thường", "Kém" });
        questionResult.IsSuccess.Should().BeTrue();

        var publishResult = campaign.PublicCampaign();
        publishResult.IsSuccess.Should().BeTrue();

        await DbContext.KhaoSats.AddAsync(campaign);
        await DbContext.SaveChangesAsync();

        return (campaign, canHo, resident);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnIsVotedTrue_When_ResidentHasVoted()
    {
        // Arrange
        var (campaign, canHo, resident) = await CreateDataAsync();

        // Create a voted ballot
        var luaChon = campaign.CauHois.First().LuaChons.First();
        var bieuQuyetResult = BieuQuyetCuDan.Create(
            campaign.Id,
            canHo.Id,
            canHo.ThongSo.DienTich,
            campaign.CoCheTinhDiemId,
            new List<(int, string?)> { (luaChon.Id, null) },
            isOtpVerified: true
        );
        bieuQuyetResult.IsSuccess.Should().BeTrue();
        var bieuQuyet = bieuQuyetResult.Value;

        await DbContext.BieuQuyetCuDans.AddAsync(bieuQuyet);
        await DbContext.SaveChangesAsync();

        var spec = new GetKhaoSatListSpecification(
            null, null, null, null, null, null, null, null, null,
            currentUserId: resident.Id
        );

        // Act
        var result = await _repository.GetAllAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        var campaignItem = result.Items.FirstOrDefault(k => k.Id == campaign.Id);
        campaignItem.Should().NotBeNull();
        campaignItem!.IsVoted.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnIsVotedFalse_When_ResidentHasNotVoted()
    {
        // Arrange
        var (campaign, _, resident) = await CreateDataAsync();

        var spec = new GetKhaoSatListSpecification(
            null, null, null, null, null, null, null, null, null,
            currentUserId: resident.Id
        );

        // Act
        var result = await _repository.GetAllAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        var campaignItem = result.Items.FirstOrDefault(k => k.Id == campaign.Id);
        campaignItem.Should().NotBeNull();
        campaignItem!.IsVoted.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnIsVotedTrue_When_Voted()
    {
        // Arrange
        var (campaign, canHo, resident) = await CreateDataAsync();

        var luaChon = campaign.CauHois.First().LuaChons.First();
        var bieuQuyetResult = BieuQuyetCuDan.Create(
            campaign.Id,
            canHo.Id,
            canHo.ThongSo.DienTich,
            campaign.CoCheTinhDiemId,
            new List<(int, string?)> { (luaChon.Id, null) },
            isOtpVerified: true
        );
        bieuQuyetResult.IsSuccess.Should().BeTrue();
        await DbContext.BieuQuyetCuDans.AddAsync(bieuQuyetResult.Value);
        await DbContext.SaveChangesAsync();

        var spec = new GetKhaoSatByIdSpecification(campaign.Id, currentUserId: resident.Id);

        // Act
        var result = await _repository.GetByIdAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(campaign.Id);
        result.IsVoted.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnIsVotedFalse_When_NotVoted()
    {
        // Arrange
        var (campaign, _, resident) = await CreateDataAsync();

        var spec = new GetKhaoSatByIdSpecification(campaign.Id, currentUserId: resident.Id);

        // Act
        var result = await _repository.GetByIdAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(campaign.Id);
        result.IsVoted.Should().BeFalse();
    }
}
