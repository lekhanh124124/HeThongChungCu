using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ToaNhaSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
    {
        if (!await context.ToaNhas.AnyAsync())
        {
            logger.LogInformation("Seeding ToaNhas...");

            var toaNhaFaker = new Faker<ToaNha>("vi")
                .CustomInstantiator(f => new ToaNha(
                    maToaNha: f.Random.Replace("TN-##"),
                    tenToaNha: $"Tòa nhà {f.Address.BuildingNumber()}",
                    diaChi: f.Address.StreetAddress(),
                    moTa: f.Lorem.Sentence(),
                    trangThaiToaNhaId: 1 // HoatDong
                ));

            var toaNhas = toaNhaFaker.Generate(count);
            await context.ToaNhas.AddRangeAsync(toaNhas);
            await context.SaveChangesAsync();
        }
    }
}
