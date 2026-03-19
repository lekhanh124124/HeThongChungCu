using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ToaNhaSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int buildingCount, int floorCount, int basementCount)
    {
        if (!await context.ToaNhas.AnyAsync())
        {
            logger.LogInformation("Seeding ToaNhas and Tangs...");

            var toaNhaFaker = new Faker<ToaNha>("vi")
                .CustomInstantiator(f => new ToaNha(
                    maToaNha: f.Random.Replace("TN-##"),
                    tenToaNha: $"Tòa nhà {f.Address.BuildingNumber()}",
                    diaChi: f.Address.StreetAddress(),
                    moTa: f.Lorem.Sentence(),
                    trangThaiToaNhaId: TrangThaiToaNha.DangHoatDong
                ));

            var toaNhas = toaNhaFaker.Generate(buildingCount);

            foreach (var toaNha in toaNhas)
            {
                // Add basements
                for (int i = 1; i <= basementCount; i++)
                {
                    toaNha.AddTang($"TN{toaNha.MaToaNha}-B{i}", $"Tầng hầm {i}", LoaiTang.TangHam);
                }

                // Add floors
                for (int i = 1; i <= floorCount; i++)
                {
                    toaNha.AddTang($"TN{toaNha.MaToaNha}-F{i}", $"Tầng {i}", LoaiTang.TangLau);
                }
            }

            await context.ToaNhas.AddRangeAsync(toaNhas);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} ToaNhas with their floors.", toaNhas.Count);
        }
    }
}
