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

        var residents = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue)
            .Select(r => new { r.CanHoId, r.ThoiGian.NgayBatDau, r.ThoiGian.NgayKetThuc })
            .ToListAsync();

        var services = await context.DichVus.Where(d => d.LoaiDichVuId == LoaiDichVu.TienIch).ToListAsync();

        if (!residents.Any() || !services.Any())
        {
            logger.LogWarning("No residents or utility services found to seed bookings.");
            return;
        }

        var random = new Random();
        var bookings = new List<DangKyDichVu>();
        var count = 80;

        for (int i = 0; i < count; i++)
        {
            var resident = residents[random.Next(residents.Count)];
            var svc = services[random.Next(services.Count)];
            
            var minDate = resident.NgayBatDau;
            var maxDate = resident.NgayKetThuc ?? DateTimeOffset.Now;
            if (minDate >= maxDate) minDate = maxDate.AddDays(-1);

            // Sinh ngày booking ngẫu nhiên trong thời gian sống
            var bookingDate = minDate.AddDays(random.Next(0, (int)(maxDate - minDate).TotalDays));

            // Random time between 8h-20h
            var dateWithTime = new DateTimeOffset(
                bookingDate.Year, bookingDate.Month, bookingDate.Day, 
                random.Next(8, 20), 0, 0, TimeSpan.FromHours(7));

            var booking = new DangKyDichVu(resident.CanHoId, svc.Id, dateWithTime, random.Next(1, 3));
            booking.UpdateStatus(TrangThaiDangKy.DangSuDung);
            
            bookings.Add(booking);
        }

        await context.DangKyDichVus.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
        logger.LogInformation($"Successfully seeded {bookings.Count} utility bookings.");
    }
}
