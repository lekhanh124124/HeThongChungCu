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
            .Where(x => x.IsBatBuoc)
            .ToListAsync();

        if (mandatoryServices.Count == 0)
        {
            logger.LogWarning("No mandatory services found to register.");
            return;
        }

        // 2. Get all apartments
        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count == 0)
        {
            logger.LogWarning("No apartments found to register services for.");
            return;
        }

        var startOfMonth = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1, 0, 0, 0, DateTimeOffset.UtcNow.Offset);

        // 3. Create registrations
        var registrations = new List<DangKyDichVu>();
        foreach (var canHo in canHos)
        {
            foreach (var dichVu in mandatoryServices)
            {
                var registration = new DangKyDichVu(canHo.Id, dichVu.Id, startOfMonth);
                registration.UpdateStatus(TrangThaiDangKy.DangSuDung);
                
                if (adminId != 0) registration.SetCreated(adminId, DateTimeOffset.UtcNow);
                
                registrations.Add(registration);
            }
        }

        await context.DangKyDichVus.AddRangeAsync(registrations);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully registered {Count} service records for {ApartmentCount} apartments.", registrations.Count, canHos.Count);
    }
}
