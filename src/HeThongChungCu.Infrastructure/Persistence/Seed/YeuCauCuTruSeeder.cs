using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class YeuCauCuTruSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        YeuCauCounts? counts)
    {
        if (counts == null) return;

        logger.LogInformation("Seeding YeuCauCuTru...");

        var faker = new Faker("vi");
        var adminAccount = await context.TaiKhoan
            .FirstOrDefaultAsync(a => a.TenDangNhap == "admin@gmail.com");

        // Get householders (both active and moved out)
        var householders = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo)
            .ToListAsync();

        if (householders.Count == 0)
        {
            logger.LogWarning("No householders found. Skipping YeuCauCuTru seeding.");
            return;
        }

        await SeedResidencyRequestsByType(context, householders, LoaiYeuCau.Them, counts.SoLuongThem, faker, adminAccount);
        await SeedResidencyRequestsByType(context, householders, LoaiYeuCau.Sua, counts.SoLuongSua, faker, adminAccount);
        await SeedResidencyRequestsByType(context, householders, LoaiYeuCau.Xoa, counts.SoLuongXoa, faker, adminAccount);

        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding YeuCauCuTru.");
    }

    private static async Task SeedResidencyRequestsByType(
        AppDbContext context,
        List<QuanHeCuTru> householders,
        LoaiYeuCau loaiYeuCau,
        int count,
        Faker faker,
        TaiKhoan? admin)
    {
        for (int i = 0; i < count; i++)
        {
            var householder = faker.PickRandom(householders);
            var initialStatus = DetermineInitialStatus(householder, faker, out var targetStatus);

            YeuCauCuTru request;
            if (loaiYeuCau == LoaiYeuCau.Them)
            {
                request = YeuCauCuTru.CreateAddMemberRequest(
                    householder.CanHoId,
                    null,
                    LoaiQuanHeCuTru.NguoiOCung.Value,
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    faker.Date.Past(30, DateTime.UtcNow.AddYears(-20)),
                    faker.PickRandom(new[] { 1, 2 }), // GioiTinh
                    UserSeeder.GetUniquePhoneNumber(),
                    UserSeeder.GetUniqueIdCard(),
                    faker.Address.FullAddress(),
                    faker.Lorem.Sentence(),
                    null,
                    initialStatus);
            }
            else if (loaiYeuCau == LoaiYeuCau.Sua)
            {
                request = YeuCauCuTru.CreateUpdateMemberRequest(
                    householder.CanHoId,
                    householder.Id,
                    LoaiQuanHeCuTru.NguoiOCung.Value,
                    householder.Id.ToString(), // Dummy
                    "SeederUpdate",
                    null,
                    null,
                    null,
                    null,
                    null,
                    faker.Lorem.Sentence(),
                    null,
                    initialStatus);
            }
            else // Xoa
            {
                request = YeuCauCuTru.CreateRemoveMemberRequest(
                    householder.CanHoId,
                    householder.Id,
                    faker.Lorem.Sentence(),
                    initialStatus);
            }

            // Apply Approval/Rejection if needed
            if (targetStatus == TrangThaiYeuCau.Approved && admin != null)
            {
                request.Approve(admin.Id, DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }
            else if (targetStatus == TrangThaiYeuCau.Rejected && admin != null)
            {
                request.Reject(admin.Id, "Hồ sơ đính kèm không đủ cơ sở pháp lý.", DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }

            await context.YeuCauCuTrus.AddAsync(request);
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
