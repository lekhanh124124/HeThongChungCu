using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DichVuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.Set<DichVu>().AnyAsync())
        {
            logger.LogInformation("Seeding DichVus...");

            var dichVus = new List<DichVu>
            {
                new DichVu("DV-DIEN", "Tiền Điện", "kWh"),
                new DichVu("DV-NUOC", "Tiền Nước", "m3"),
                new DichVu("DV-GUIXE", "Phí gửi xe", "Tháng"),
                new DichVu("DV-QL", "Phí quản lý", "m2"),
                new DichVu("DV-RAC", "Rác", "Tháng"),
                new DichVu("DV-INTERNET", "Internet", "Tháng")
            };

            await context.Set<DichVu>().AddRangeAsync(dichVus);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} DichVus.", dichVus.Count);
        }
    }
}
