using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ChiSoTieuThuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.Set<ChiSoTieuThu>().AnyAsync())
        {
            logger.LogInformation("Seeding ChiSoTieuThus...");

            var canHoIds = await context.CanHos.Select(c => c.Id).ToListAsync();

            if (canHoIds.Any())
            {
                var faker = new Faker("vi");
                var chiSoTieuThus = new List<ChiSoTieuThu>();

                foreach (var canHoId in canHoIds)
                {
                    // Seed for last 3 months
                    for (int i = 0; i < 3; i++)
                    {
                        var date = DateTime.Now.AddMonths(-i);
                        
                        // Water
                        chiSoTieuThus.Add(new ChiSoTieuThu(
                            canHoId,
                            LoaiDichVu.Nuoc,
                            faker.Random.Double(10, 50),
                            date.Month,
                            date.Year,
                            new DateTime(date.Year, date.Month, 25)
                        ));

                        // Electricity
                        chiSoTieuThus.Add(new ChiSoTieuThu(
                            canHoId,
                            LoaiDichVu.Dien,
                            faker.Random.Double(100, 500),
                            date.Month,
                            date.Year,
                            new DateTime(date.Year, date.Month, 25)
                        ));
                    }
                }

                await context.Set<ChiSoTieuThu>().AddRangeAsync(chiSoTieuThus);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} ChiSoTieuThus.", chiSoTieuThus.Count);
            }
        }
    }
}
