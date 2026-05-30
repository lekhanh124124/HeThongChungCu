using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DangKyDichVuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.DangKyDichVus.AnyAsync()) return;

        logger.LogInformation("Seeding utility bookings (DangKyDichVu)...");

        var apartments = await context.CanHos.ToListAsync();
        var services = await context.DichVus.Where(d => d.LoaiDichVuId == LoaiDichVu.TienIch).ToListAsync();

        if (!apartments.Any() || !services.Any())
        {
            logger.LogWarning("No apartments or utility services found to seed bookings.");
            return;
        }

        var random = new Random();
        var bookings = new List<DangKyDichVu>();

        // Generate bookings in Month 3, 4, 5 of 2026
        var dates = new List<DateTimeOffset>();
        
        // Month 3
        for (int i = 0; i < 20; i++)
            dates.Add(new DateTimeOffset(2026, 3, random.Next(1, 29), random.Next(8, 20), 0, 0, TimeSpan.FromHours(7)));
        
        // Month 4
        for (int i = 0; i < 25; i++)
            dates.Add(new DateTimeOffset(2026, 4, random.Next(1, 29), random.Next(8, 20), 0, 0, TimeSpan.FromHours(7)));
            
        // Month 5
        // Ensure some bookings are today (May 25, 2026)
        for (int i = 0; i < 5; i++)
            dates.Add(new DateTimeOffset(2026, 5, 25, random.Next(8, 20), 0, 0, TimeSpan.FromHours(7)));
        
        // Other days in Month 5
        for (int i = 0; i < 30; i++)
            dates.Add(new DateTimeOffset(2026, 5, random.Next(1, 25), random.Next(8, 20), 0, 0, TimeSpan.FromHours(7)));

        foreach (var date in dates)
        {
            var apt = apartments[random.Next(apartments.Count)];
            var svc = services[random.Next(services.Count)];
            
            var booking = new DangKyDichVu(apt.Id, svc.Id, date, random.Next(1, 3));
            booking.UpdateStatus(TrangThaiDangKy.DangSuDung);
            
            bookings.Add(booking);
        }

        await context.DangKyDichVus.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
        logger.LogInformation($"Successfully seeded {bookings.Count} utility bookings.");
    }
}
