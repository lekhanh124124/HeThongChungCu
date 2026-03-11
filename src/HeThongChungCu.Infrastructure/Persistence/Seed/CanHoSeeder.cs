using Bogus;
using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class CanHoSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.CanHos.AnyAsync())
        {
            logger.LogInformation("Seeding CanHos...");

            var tangs = await context.Tangs
                .Where(t => t.LoaiTangId != LoaiTang.TangHam.Value)
                .ToListAsync();
            var allCanHos = new List<CanHo>();

            foreach (var tang in tangs)
            {
                int apartmentIndex = 1;
                var canHoFaker = new Faker<CanHo>("vi")
                    .CustomInstantiator(f =>
                    {
                        var maCanHo = $"{tang.MaTang}-{apartmentIndex++:D2}";
                        return new CanHo(
                            maCanHo: maCanHo,
                            dienTich: f.Random.Decimal(45m, 120m),
                            tangId: tang.Id,
                            soPhongNgu: f.Random.Int(1, 3),
                            soPhongTam: f.Random.Int(1, 2),
                            loaiCanHoId: f.Random.Int(1, 3),
                            tinhTrangCanHoId: f.Random.Int(1, 2)
                        );
                    });

                var canHos = canHoFaker.Generate(new Random().Next(4, 7));
                allCanHos.AddRange(canHos);
            }

            await context.CanHos.AddRangeAsync(allCanHos);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} CanHos across {FloorCount} floors.", allCanHos.Count, tangs.Count);
        }
    }
}
