using Bogus;
using HeThongChungCu.Domain.Entities.PhuongTien;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class PhuongTienSeeder
{
    public static async Task SeedAsync(EFDbContext context, ILogger logger)
    {
        if (!await context.Set<PhuongTien>().AnyAsync())
        {
            logger.LogInformation("Seeding PhuongTiens...");

            var canHoIds = await context.CanHos.Select(c => c.Id).ToListAsync();

            if (canHoIds.Any())
            {
                var phuongTienFaker = new Faker<PhuongTien>("vi")
                    .CustomInstantiator(f => new PhuongTien(
                        canHoId: f.PickRandom(canHoIds),
                        tenPhuongTien: f.Vehicle.Model(),
                        loaiPhuongTienId: f.Random.Int(1, 2), // 1: Xe máy, 2: Ô tô
                        bienSo: $"{f.Random.Int(11, 99)}{f.Random.String2(1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{f.Random.Int(10000, 99999)}",
                        mauXe: f.Commerce.Color()
                    ));

                var phuongTiens = phuongTienFaker.Generate(30);

                // Add some cards to a few vehicles
                var faker = new Faker("vi");
                foreach (var pt in phuongTiens.Take(20))
                {
                    pt.AddThe($"CARD-{faker.Random.Number(100000, 999999)}", faker.Date.Past(1));
                }

                await context.Set<PhuongTien>().AddRangeAsync(phuongTiens);
                await context.SaveChangesAsync();
            }
        }
    }
}
