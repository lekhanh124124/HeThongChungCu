using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ChiSoTieuThuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.Set<ChiSoTieuThu>().AnyAsync())
        {
            logger.LogInformation("Seeding ChiSoTieuThus...");

            var canHos = await context.CanHos.ToListAsync();
            var dichVuDien = await context.Set<DichVu>().FirstOrDefaultAsync(x => x.MaDichVu == "DV-DIEN");
            var dichVuNuoc = await context.Set<DichVu>().FirstOrDefaultAsync(x => x.MaDichVu == "DV-NUOC");

            if (canHos.Any() && dichVuDien != null && dichVuNuoc != null)
            {
                var faker = new Faker("vi");
                var allChiSos = new List<ChiSoTieuThu>();

                foreach (var canHo in canHos)
                {
                    // Seed for 3 months
                    double currentDien = faker.Random.Double(100, 500);
                    double currentNuoc = faker.Random.Double(10, 50);

                    for (int month = 1; month <= 3; month++)
                    {
                        double nextDien = currentDien + faker.Random.Double(50, 150);
                        double nextNuoc = currentNuoc + faker.Random.Double(5, 15);

                        var csDien = new ChiSoTieuThu(
                            canHo.Id,
                            dichVuDien.Id,
                            currentDien,
                            nextDien,
                            month,
                            2024,
                            new DateTime(2024, month, 1).AddMonths(1).AddDays(-1)
                        );
                        csDien.Lock();

                        var csNuoc = new ChiSoTieuThu(
                            canHo.Id,
                            dichVuNuoc.Id,
                            currentNuoc,
                            nextNuoc,
                            month,
                            2024,
                            new DateTime(2024, month, 1).AddMonths(1).AddDays(-1)
                        );
                        csNuoc.Lock();

                        allChiSos.Add(csDien);
                        allChiSos.Add(csNuoc);

                        currentDien = nextDien;
                        currentNuoc = nextNuoc;
                    }
                }

                await context.Set<ChiSoTieuThu>().AddRangeAsync(allChiSos);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} ChiSoTieuThus.", allChiSos.Count);
            }
        }
    }
}
