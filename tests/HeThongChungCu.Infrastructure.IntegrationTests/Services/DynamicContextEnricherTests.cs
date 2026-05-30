using FluentAssertions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.IntegrationTests.Persistence.Repositories;
using HeThongChungCu.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Services;

public class DynamicContextEnricherTests : BaseIntegrationTest
{
    private readonly DynamicContextEnricher _enricher;

    public DynamicContextEnricherTests() : base()
    {
        var logger = Substitute.For<ILogger<DynamicContextEnricher>>();
        _enricher = new DynamicContextEnricher(DbContext, logger);
    }

    [Fact]
    public async Task EnrichAsync_WithDichVuIntent_ShouldReturnActiveServices()
    {
        // Arrange
        // 1. Active Service (TrangThaiId = HoatDong / 1)
        var serviceActive1 = new DichVu("DV001", "Dịch vụ Vệ sinh", LoaiDichVu.VanHanh, "Tháng", "Vệ sinh hành lang tòa nhà", null, false);
        serviceActive1.Activate();
        
        var serviceActive2 = new DichVu("DV002", "Dịch vụ Bảo vệ", LoaiDichVu.VanHanh, "Tháng", "Bảo vệ an ninh 24/7", null, true);
        serviceActive2.Activate();

        // 2. Inactive Service (TrangThaiId = TaoMoi / 4)
        var serviceInactive = new DichVu("DV003", "Dịch vụ Gym", LoaiDichVu.TienIch, "Tháng", "Phòng tập Gym", null, false);

        // 3. Deleted Service (IsDeleted = true)
        var serviceDeleted = new DichVu("DV004", "Dịch vụ Hồ bơi", LoaiDichVu.TienIch, "Tháng", "Hồ bơi vô cực", null, false);
        serviceDeleted.Activate();
        serviceDeleted.MarkAsDeleted(DateTimeOffset.UtcNow); // Soft delete

        await DbContext.DichVus.AddRangeAsync(serviceActive1, serviceActive2, serviceInactive, serviceDeleted);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _enricher.EnrichAsync("Tôi muốn tìm hiểu các dịch vụ có ở chung cư mình");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain("[DỮ LIỆU THỜI GIAN THỰC");
        result.Should().Contain("Danh sách dịch vụ đang cung cấp");
        
        // Active services should be listed
        result.Should().Contain("Dịch vụ Vệ sinh");
        result.Should().Contain("Dịch vụ Bảo vệ");
        result.Should().Contain("(bắt buộc)"); // Service 2 isBatBuoc = true
        result.Should().Contain("(tùy chọn)"); // Service 1 isBatBuoc = false
        
        // Inactive and deleted services should not be listed
        result.Should().NotContain("Dịch vụ Gym");
        result.Should().NotContain("Dịch vụ Hồ bơi");
    }

    [Fact]
    public async Task EnrichAsync_WithBangGiaIntent_ShouldReturnActiveServicesAndValidPrices()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // 1. Service with active price list
        var service1 = new DichVu("DV010", "Giữ xe máy", LoaiDichVu.TienIch, "Chiếc", "Giữ xe máy cư dân", null, false);
        service1.Activate();
        service1.AddBangGiaCoDinh("Bảng giá giữ xe máy 2026", now.AddDays(-5), 120000, true, LoaiDinhGia.CoDinh, null);
        
        // Activate price lists inside service
        var bgActive = service1.BangGias.First();
        bgActive.Activate();

        // 2. Service with expired price list
        var service2 = new DichVu("DV011", "Vệ sinh đặc biệt", LoaiDichVu.YeuCau, "Lần", "Vệ sinh theo yêu cầu", null, false);
        service2.Activate();
        service2.AddBangGiaCoDinh("Bảng giá vệ sinh cũ", now.AddDays(-20), 500000, false, LoaiDinhGia.CoDinh, now.AddDays(-5));
        var bgExpired = service2.BangGias.First();
        bgExpired.Activate();

        // 3. Service with future price list (not effective yet)
        var service3 = new DichVu("DV012", "Dịch vụ giặt ủi", LoaiDichVu.YeuCau, "Kg", "Giặt sấy quần áo", null, false);
        service3.Activate();
        service3.AddBangGiaCoDinh("Bảng giá tương lai", now.AddDays(5), 30000, true, LoaiDinhGia.CoDinh, null);
        var bgFuture = service3.BangGias.First();
        bgFuture.Activate();

        await DbContext.DichVus.AddRangeAsync(service1, service2, service3);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _enricher.EnrichAsync("Cho hỏi bảng giá dịch vụ giữ xe của tòa nhà");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain("Bảng giá hiện hành");
        
        // Active and current price should exist
        result.Should().Contain("**Giữ xe máy**: 120,000 VNĐ (Bảng giá giữ xe máy 2026)");
        
        // Expired or future prices should not be in the output
        result.Should().NotContain("Bảng giá vệ sinh cũ");
        result.Should().NotContain("Bảng giá tương lai");
    }

    [Fact]
    public async Task EnrichAsync_WithThongBaoIntent_ShouldReturnRecentNotifications()
    {
        // Arrange
        // 1. Notification in last 7 days (System - should be returned)
        var thongBaoRecent = new ThongBao("Thông báo cắt nước định kỳ", "Tòa nhà sẽ tạm ngừng cấp nước vào sáng chủ nhật", LoaiThongBao.HeThong);
        
        // 2. Notification older than 7 days (System - should not be returned)
        var thongBaoOld = new ThongBao("Thông tin họp cư dân", "Nội dung họp cư dân tháng trước", LoaiThongBao.HeThong);

        // 3. Deleted notification (System - should not be returned)
        var thongBaoDeleted = new ThongBao("Thông báo nhầm lẫn", "Nội dung này đã bị xóa", LoaiThongBao.HeThong);
        thongBaoDeleted.MarkAsDeleted(DateTimeOffset.UtcNow); // Soft delete

        // 4. Non-system notification in last 7 days (should not be returned)
        var thongBaoOtherType = new ThongBao("Thông báo nợ phí dịch vụ", "Vui lòng hoàn thành thanh toán", LoaiThongBao.ThanhToan);

        await DbContext.ThongBaos.AddRangeAsync(thongBaoRecent, thongBaoOld, thongBaoDeleted, thongBaoOtherType);
        await DbContext.SaveChangesAsync();

        // Workaround to set correct CreatedAt since EF interceptor sets it automatically to DateTimeOffset.Now.
        await DbContext.Database.ExecuteSqlRawAsync($"UPDATE ThongBao SET CreatedAt = '{DateTimeOffset.Now.AddDays(-10):yyyy-MM-dd HH:mm:ss.ffffff zzz}' WHERE Id = {thongBaoOld.Id}");
        await DbContext.Database.ExecuteSqlRawAsync($"UPDATE ThongBao SET CreatedAt = '{DateTimeOffset.Now.AddDays(-2):yyyy-MM-dd HH:mm:ss.ffffff zzz}' WHERE Id = {thongBaoRecent.Id}");
        await DbContext.Database.ExecuteSqlRawAsync($"UPDATE ThongBao SET CreatedAt = '{DateTimeOffset.Now.AddDays(-1):yyyy-MM-dd HH:mm:ss.ffffff zzz}' WHERE Id = {thongBaoOtherType.Id}");

        // Act
        var result = await _enricher.EnrichAsync("Xem giùm tôi có thông báo gì mới gần đây không");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain("Thông báo gần đây (7 ngày qua)");
        result.Should().Contain("Thông báo cắt nước định kỳ");
        
        // Old, deleted, and non-system notifications should not be listed
        result.Should().NotContain("Thông tin họp cư dân");
        result.Should().NotContain("Thông báo nhầm lẫn");
        result.Should().NotContain("Thông báo nợ phí dịch vụ");
    }

    [Fact]
    public async Task EnrichAsync_WithMultipleIntents_ShouldReturnCombinedContext()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        
        // 1. Service & Price
        var service = new DichVu("DV020", "Phí Quản Lý", LoaiDichVu.VanHanh, "m2", "Phí quản lý vận hành chung cư", null, true);
        service.Activate();
        service.AddBangGiaCoDinh("Đơn giá quản lý 2026", now.AddDays(-2), 10000, true, LoaiDinhGia.CoDinh, null);
        service.BangGias.First().Activate();

        // 2. Notification (Must be LoaiThongBao.HeThong to be selected by the enricher)
        var thongBao = new ThongBao("Thông báo nộp phí quản lý", "Vui lòng hoàn thành phí quản lý trước ngày 5 hàng tháng", LoaiThongBao.HeThong);

        await DbContext.DichVus.AddAsync(service);
        await DbContext.ThongBaos.AddAsync(thongBao);
        await DbContext.SaveChangesAsync();

        await DbContext.Database.ExecuteSqlRawAsync($"UPDATE ThongBao SET CreatedAt = '{DateTimeOffset.Now.AddDays(-1):yyyy-MM-dd HH:mm:ss.ffffff zzz}' WHERE Id = {thongBao.Id}");

        // Act
        var result = await _enricher.EnrichAsync("Có thông báo gì về dịch vụ quản lý và bảng giá phí quản lý không?");

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain("Danh sách dịch vụ đang cung cấp");
        result.Should().Contain("Bảng giá hiện hành");
        result.Should().Contain("Thông báo gần đây (7 ngày qua)");
        
        result.Should().Contain("Phí Quản Lý");
        result.Should().Contain("10,000 VNĐ");
        result.Should().Contain("Thông báo nộp phí quản lý");
    }

    [Fact]
    public async Task EnrichAsync_WithNoMatchingIntent_ShouldReturnEmptyString()
    {
        // Arrange
        var service = new DichVu("DV030", "Dịch vụ Test", LoaiDichVu.VanHanh, "Lần", "Mô tả", null, false);
        service.Activate();
        await DbContext.DichVus.AddAsync(service);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _enricher.EnrichAsync("Chào bạn, hôm nay thời tiết thế nào?");

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task EnrichAsync_WithWhitespacePrompt_ShouldReturnEmptyString(string prompt)
    {
        // Act
        var result = await _enricher.EnrichAsync(prompt);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task EnrichAsync_WithNullPrompt_ShouldReturnEmptyString()
    {
        // Act
        var result = await _enricher.EnrichAsync(null!);

        // Assert
        result.Should().BeEmpty();
    }
}
