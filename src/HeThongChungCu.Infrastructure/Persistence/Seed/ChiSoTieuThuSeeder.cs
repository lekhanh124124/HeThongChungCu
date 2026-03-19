using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ChiSoTieuThuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int soLuongChiSoTieuThuMoiCanHo)
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
                    double currentWater = faker.Random.Double(10, 50);
                    double currentElectricity = faker.Random.Double(50, 200);

                    // Seed from oldest to newest month to ensure cumulative values
                    for (int i = soLuongChiSoTieuThuMoiCanHo - 1; i >= 0; i--)
                    {
                        var date = DateTime.Now.AddMonths(-i);
                        
                        // Increment by random amount
                        currentWater += faker.Random.Double(5, 20);
                        currentElectricity += faker.Random.Double(50, 150);

                        // Water
                        chiSoTieuThus.Add(new ChiSoTieuThu(
                            canHoId,
                            LoaiDichVu.Nuoc,
                            currentWater,
                            date.Month,
                            date.Year,
                            new DateTime(date.Year, date.Month, 25)
                        ));

                        // Electricity
                        chiSoTieuThus.Add(new ChiSoTieuThu(
                            canHoId,
                            LoaiDichVu.Dien,
                            currentElectricity,
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
