using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DangKyDichVuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.DangKyDichVus.AnyAsync())
        {
            logger.LogInformation("Registrations already exist. Skipping DangKyDichVuSeeder.");
            return;
        }

        logger.LogInformation("Seeding Mandatory Service Registrations for all apartments...");

        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        // 1. Get all mandatory services
        var mandatoryServices = await context.DichVus
            .Where(x => x.IsBatBuoc && x.TrangThaiId == TrangThaiDichVu.HoatDong)
            .ToListAsync();

        if (mandatoryServices.Count == 0)
        {
            logger.LogWarning("No active mandatory services found to register.");
            return;
        }

        // 2. Get all apartments and their ChuHo accounts
        var apartmentChuHos = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .Join(context.TaiKhoan, 
                qh => qh.NguoiDungId, 
                tk => tk.NguoiDungId, 
                (qh, tk) => new { qh.CanHoId, tk.Id })
            .GroupBy(x => x.CanHoId)
            .ToDictionaryAsync(g => g.Key, g => g.First().Id);

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count == 0)
        {
            logger.LogWarning("No apartments found to register services for.");
            return;
        }

        var startOfMonth = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, DateTimeOffset.Now.Offset);

        // 3. Create registrations
        var registrations = new List<DangKyDichVu>();
        foreach (var canHo in canHos)
        {
            // Use ChuHo as creator if exists, otherwise default to admin
            int creatorId = apartmentChuHos.TryGetValue(canHo.Id, out var chuHoId) ? chuHoId : adminId;

            foreach (var dichVu in mandatoryServices)
            {
                var registration = new DangKyDichVu(canHo.Id, dichVu.Id, startOfMonth);
                registration.UpdateStatus(TrangThaiDangKy.DangSuDung);

                if (creatorId != 0) 
                {
                    // If creator is ChuHo, they probably joined earlier
                    var createdDate = apartmentChuHos.ContainsKey(canHo.Id) ? startOfMonth.AddHours(8) : DateTimeOffset.Now;
                    registration.SetCreated(creatorId, createdDate);
                }

                registrations.Add(registration);
            }
        }

        await context.DangKyDichVus.AddRangeAsync(registrations);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully registered {Count} service records for {ApartmentCount} apartments.", registrations.Count, canHos.Count);
    }
}
