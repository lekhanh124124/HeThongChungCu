using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class TangSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
    {
        if (!await context.Tangs.AnyAsync())
        {
            logger.LogInformation("Seeding Tangs...");

            var toaNhaIds = await context.ToaNhas.Select(t => t.Id).ToListAsync();

            foreach (var toaNhaId in toaNhaIds)
            {
                var tangs = new List<Tang>();

                // Seed basements
                for (int i = 1; i <= 2; i++)
                {
                    tangs.Add(new Tang(
                        maTang: $"TN{toaNhaId}-B{i}",
                        tenTang: $"Tầng hầm {i}",
                        loaiTangId: LoaiTang.TangHam.Value,
                        toaNhaId: toaNhaId
                    ));
                }

                // Seed floors
                for (int i = 1; i <= count; i++)
                {
                    tangs.Add(new Tang(
                        maTang: $"TN{toaNhaId}-F{i}",
                        tenTang: $"Tầng {i}",
                        loaiTangId: LoaiTang.TangLau.Value,
                        toaNhaId: toaNhaId
                    ));
                }

                await context.Tangs.AddRangeAsync(tangs);
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} Tangs.", toaNhaIds.Count * (2 + count)); // basements + floors per building
        }
    }
}
