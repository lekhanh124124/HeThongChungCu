using Bogus;
using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class CanHoSeeder
{
    public static async Task SeedAsync(EFDbContext context, ILogger logger)
    {
        if (!await context.CanHos.AnyAsync())
        {
            logger.LogInformation("Seeding CanHos...");

            var toaNhaIds = await context.ToaNhas.Select(t => t.Id).ToListAsync();

            var canHoFaker = new Faker<CanHo>("vi")
                .CustomInstantiator(f => new CanHo(
                    toaNhaId: f.PickRandom(toaNhaIds),
                    maCanHo: f.Random.Replace("A-###"),
                    dienTich: f.Random.Decimal(45m, 120m),
                    tang: f.Random.Int(1, 20),
                    soPhongNgu: f.Random.Int(1, 3),
                    soPhongTam: f.Random.Int(1, 2),
                    loaiCanHoId: f.Random.Int(1, 3),
                    tinhTrangCanHoId: f.Random.Int(1, 2)
                ));

            var canHos = canHoFaker.Generate(20);
            await context.CanHos.AddRangeAsync(canHos);
            await context.SaveChangesAsync();
        }
    }
}
