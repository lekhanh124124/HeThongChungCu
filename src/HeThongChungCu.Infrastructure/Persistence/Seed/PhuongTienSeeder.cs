using Bogus;
using HeThongChungCu.Domain.Entities.PhuongTien;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class PhuongTienSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
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
                        loaiPhuongTienId: f.PickRandom(LoaiPhuongTien.GetAll().ToArray()),
                        bienSo: $"{f.Random.Int(11, 99)}{f.Random.String2(1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{f.Random.Int(10000, 99999)}",
                        mauXe: f.Commerce.Color()
                    ));

                var phuongTiens = phuongTienFaker.Generate(count);

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
