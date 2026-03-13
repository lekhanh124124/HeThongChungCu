using Bogus;
using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class CanHoSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
    {
        if (!await context.CanHos.AnyAsync())
        {
            logger.LogInformation("Seeding CanHos...");

            var tangs = await context.Tangs
                .Where(t => t.LoaiTangId != LoaiTang.TangHam)
                .ToListAsync();
            var allCanHos = new List<CanHo>();

            foreach (var tang in tangs)
            {
                int apartmentIndex = 1;
                var canHoFaker = new Faker<CanHo>("vi")
                    .CustomInstantiator(f =>
                    {
                        var maCanHo = $"{tang.MaTang}-{apartmentIndex++:D2}";
                        var tenCanHo = $"Căn hộ {maCanHo}";
                        return new CanHo(
                            maCanHo: maCanHo,
                            tenCanHo: tenCanHo,
                            dienTich: f.Random.Decimal(45m, 120m),
                            tangId: tang.Id,
                            soPhongNgu: f.Random.Int(1, 3),
                            soPhongTam: f.Random.Int(1, 2),
                            loaiCanHoId: f.PickRandom(LoaiCanHo.GetAll().ToArray()),
                            tinhTrangCanHoId: f.PickRandom(TinhTrangCanHo.GetAll().ToArray())
                        );
                    });

                var canHos = canHoFaker.Generate(count);
                allCanHos.AddRange(canHos);
            }

            await context.CanHos.AddRangeAsync(allCanHos);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} CanHos across {FloorCount} floors.", allCanHos.Count, tangs.Count);
        }
    }
}
