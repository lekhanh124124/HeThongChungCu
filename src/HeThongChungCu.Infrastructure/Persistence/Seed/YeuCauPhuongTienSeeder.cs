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

        // Get householders (both active and moved out)
        var householders = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo)
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

        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding YeuCauPhuongTien.");
    }

    private static async Task SeedVehicleRequestsByType(
        AppDbContext context,
        List<QuanHeCuTru> householders,
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
                request = YeuCauPhuongTien.CreateAddRequest(
                    householder.CanHoId,
                    loaiXe,
                    faker.Vehicle.Model(),
                    faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                    faker.Commerce.Color(),
                    faker.Lorem.Sentence(),
                    null,
                    initialStatus);
            }
            else
            {
                // For Update/Delete, try to find a vehicle for this apartment if possible, else random
                var vehicleId = existingVehicles.Count > 0
                    ? faker.PickRandom(existingVehicles).Id
                    : faker.Random.Number(1, 100);

                if (loaiYeuCau == LoaiYeuCau.Sua)
                {
                    request = YeuCauPhuongTien.CreateUpdateRequest(
                        householder.CanHoId,
                        vehicleId,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.Lorem.Sentence(),
                        null,
                        initialStatus);
                }
                else // Xoa
                {
                    request = YeuCauPhuongTien.CreateDeleteRequest(
                        householder.CanHoId,
                        vehicleId,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.Lorem.Sentence(),
                        initialStatus);
                }
            }

            if (targetStatus == TrangThaiYeuCau.Approved && admin != null)
            {
                request.Approve(admin.Id, DateTimeOffset.UtcNow.AddDays(-faker.Random.Number(1, 5)));
            }
            else if (targetStatus == TrangThaiYeuCau.Rejected && admin != null)
            {
                request.Reject(admin.Id, "Biển số xe không rõ ràng hoặc đã tồn tại trong hệ thống.", DateTimeOffset.UtcNow.AddDays(-faker.Random.Number(1, 5)));
            }

            await context.YeuCauPhuongTiens.AddAsync(request);
        }
    }

    private static TrangThaiYeuCau DetermineInitialStatus(QuanHeCuTru householder, Faker faker, out TrangThaiYeuCau targetStatus)
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
}
