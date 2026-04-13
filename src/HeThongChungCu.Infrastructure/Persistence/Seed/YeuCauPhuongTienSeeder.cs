using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class YeuCauPhuongTienSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        YeuCauCounts? counts)
    {
        if (counts == null) return;

        logger.LogInformation("Seeding YeuCauPhuongTien...");

        var faker = new Faker("vi");
        var adminAccount = await context.TaiKhoan
            .FirstOrDefaultAsync(a => a.TenDangNhap == "admin@gmail.com");

        // Get householders (both active and moved out) with their TaiKhoanId
        var householders = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo)
            .Join(context.TaiKhoan,
                qh => qh.NguoiDungId,
                tk => tk.NguoiDungId,
                (qh, tk) => new HouseholderData
                {
                    Id = qh.Id,
                    CanHoId = qh.CanHoId,
                    TaiKhoanId = tk.Id,
                    TrangThaiCuTruId = qh.TrangThaiCuTruId
                })
            .ToListAsync();

        if (householders.Count == 0)
        {
            logger.LogWarning("No householders found. Skipping YeuCauPhuongTien seeding.");
            return;
        }

        // Get some existing vehicles for Update/Delete requests
        var existingVehicles = await context.PhuongTiens.Take(50).ToListAsync();

        await SeedVehicleRequestsByType(context, householders, existingVehicles, LoaiYeuCau.Them, counts.SoLuongThem, faker, adminAccount);
        await SeedVehicleRequestsByType(context, householders, existingVehicles, LoaiYeuCau.Sua, counts.SoLuongSua, faker, adminAccount);
        await SeedVehicleRequestsByType(context, householders, existingVehicles, LoaiYeuCau.Xoa, counts.SoLuongXoa, faker, adminAccount);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding YeuCauPhuongTien.");
    }

    private static async Task SeedVehicleRequestsByType(
        AppDbContext context,
        List<HouseholderData> householders,
        List<PhuongTien> existingVehicles,
        LoaiYeuCau loaiYeuCau,
        int count,
        Faker faker,
        TaiKhoan? admin)
    {
        var motorbikeTypes = new[] { LoaiPhuongTien.XeMay, LoaiPhuongTien.Oto, LoaiPhuongTien.XeDap, LoaiPhuongTien.XeDien };

        for (int i = 0; i < count; i++)
        {
            var householder = faker.PickRandom(householders);
            var initialStatus = DetermineInitialStatus(householder, faker, out var targetStatus);
            var loaiXe = faker.PickRandom(motorbikeTypes);

            YeuCauPhuongTien request;
            if (loaiYeuCau == LoaiYeuCau.Them)
            {
                var addContents = new[]
                {
                    "Đăng ký xe mới mua, loại sedan 5 chỗ để đi làm.",
                    "Đăng ký thêm thẻ gửi xe máy cho con mới đi học đại học.",
                    "Đăng ký chỗ đậu xe ô tô cố định dưới hầm B1.",
                    "Đăng ký sạc điện cho xe máy điện mới mua, cần vị trí gần trạm sạc.",
                    "Đăng ký xe đạp điện mới để đưa đón con đi học.",
                    "Bổ sung xe ô tô thứ 2 cho gia đình (xe SUV 7 chỗ)."
                };
                request = YeuCauPhuongTien.CreateAddRequest(
                    householder.CanHoId,
                    loaiXe,
                    faker.Vehicle.Model(),
                    faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                    faker.Commerce.Color(),
                    faker.PickRandom(addContents),
                    null,
                    initialStatus);
            }
            else
            {
                // For Update/Delete, try to find a vehicle from existing ones
                if (existingVehicles.Count == 0) continue;
                var vehicleId = faker.PickRandom(existingVehicles).Id;

                if (loaiYeuCau == LoaiYeuCau.Sua)
                {
                    var updateContents = new[]
                    {
                        "Cập nhật lại biển số xe mới sau khi làm thủ tục sang tên đổi chủ.",
                        "Sửa đổi thông tin màu sơn xe thực tế (đã dán decal đổi màu).",
                        "Cập nhật dòng xe chính xác hơn theo giấy tờ đăng ký xe.",
                        "Đính chính lại số khung, số máy do bị nhầm lẫn khi đăng ký lần đầu.",
                        "Chuyển đổi từ xe xăng sang xe điện, cần đăng ký lại dịch vụ sạc."
                    };
                    request = YeuCauPhuongTien.CreateUpdateRequest(
                        householder.CanHoId,
                        vehicleId,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.PickRandom(updateContents),
                        null,
                        initialStatus);
                }
                else // Xoa
                {
                    var removeContents = new[]
                    {
                        "Hủy thẻ gửi xe do đã bán phương tiện cho người khác.",
                        "Hết nhu cầu gửi xe ô tô tại chung cư do đã có chỗ gửi ngoài.",
                        "Xóa thông tin xe máy cũ đã hư hỏng, không còn sử dụng.",
                        "Hủy dịch vụ sạc xe điện do đã thanh lý xe.",
                        "Gia đình chuyển nhà đi nơi khác, cần hủy toàn bộ thẻ xe."
                    };
                    request = YeuCauPhuongTien.CreateDeleteRequest(
                        householder.CanHoId,
                        vehicleId,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.PickRandom(removeContents),
                        initialStatus);
                }
            }

            // Set the requester (CreatedBy) manually for seed data
            request.SetCreated(householder.TaiKhoanId, DateTimeOffset.Now.AddDays(-faker.Random.Number(5, 10)));

            if (targetStatus == TrangThaiYeuCau.Approved && admin != null)
            {
                request.Approve(admin.Id, DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }
            else if (targetStatus == TrangThaiYeuCau.Rejected && admin != null)
            {
                var rejectionReasons = new[]
                {
                    "Biển số xe không rõ ràng hoặc hình ảnh cung cấp bị lóa mờ.",
                    "Vượt quá số lượng phương tiện tối đa cho phép của một căn hộ.",
                    "Loại xe không được phép gửi trong hầm tòa nhà theo quy định.",
                    "Giấy tờ xe (Cavet) không chính chủ hoặc thiếu thông tin hợp lệ.",
                    "Biển số xe đã được đăng ký cho một căn hộ khác trong hệ thống."
                };
                request.Reject(admin.Id, faker.PickRandom(rejectionReasons), DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }

            await context.YeuCauPhuongTiens.AddAsync(request);
        }
    }

    private static TrangThaiYeuCau DetermineInitialStatus(HouseholderData householder, Faker faker, out TrangThaiYeuCau targetStatus)
    {
        if (householder.TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
        {
            targetStatus = TrangThaiYeuCau.Invalidated;
            return TrangThaiYeuCau.Invalidated;
        }

        // 60% Approved, 20% Pending, 10% Rejected, 5% Saved, 5% Withdrawn
        var rand = faker.Random.Number(1, 100);

        if (rand <= 60)
        {
            targetStatus = TrangThaiYeuCau.Approved;
            return TrangThaiYeuCau.Pending; // Needs to be Pending to call Approve()
        }

        if (rand <= 80)
        {
            targetStatus = TrangThaiYeuCau.Pending;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 90)
        {
            targetStatus = TrangThaiYeuCau.Rejected;
            return TrangThaiYeuCau.Pending; // Needs to be Pending to call Reject()
        }

        if (rand <= 95)
        {
            targetStatus = TrangThaiYeuCau.Saved;
            return TrangThaiYeuCau.Saved;
        }

        targetStatus = TrangThaiYeuCau.Withdrawn;
        return TrangThaiYeuCau.Saved; // Can be withdrawn from Saved
    }
    private class HouseholderData
    {
        public int Id { get; set; }
        public int CanHoId { get; set; }
        public int TaiKhoanId { get; set; }
        public TrangThaiCuTru TrangThaiCuTruId { get; set; } = null!;
    }
}
