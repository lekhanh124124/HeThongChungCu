using Bogus;
using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ToaNhaSeeder
{
    public static async Task SeedAsync(EFDbContext context, ILogger logger)
    {
        if (!await context.ToaNhas.AnyAsync())
        {
            logger.LogInformation("Seeding ToaNhas...");

            var toaNhaFaker = new Faker<ToaNha>("vi")
                .CustomInstantiator(f => new ToaNha(
                    maToaNha: f.Random.Replace("TN-##"),
                    tenToaNha: $"Tòa nhà {f.Address.BuildingNumber()}",
                    soTang: f.Random.Int(10, 30),
                    soTangHam: f.Random.Int(1, 3),
                    diaChi: f.Address.StreetAddress(),
                    moTa: f.Lorem.Sentence(),
                    trangThaiToaNhaId: 1 // HoatDong
                ));

            var toaNhas = toaNhaFaker.Generate(3);
            await context.ToaNhas.AddRangeAsync(toaNhas);
            await context.SaveChangesAsync();
        }
    }
}
