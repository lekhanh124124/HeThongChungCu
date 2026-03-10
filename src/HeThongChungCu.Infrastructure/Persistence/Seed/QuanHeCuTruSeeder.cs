using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class QuanHeCuTruSeeder
{
    public static async Task SeedAsync(EFDbContext context, ILogger logger)
    {
        if (!await context.QuanHeCuTrus.AnyAsync())
        {
            logger.LogInformation("Seeding QuanHeCuTrus...");

            var canHos = await context.CanHos.ToListAsync();
            var userIds = await context.Users.Where(u => u.Username != "admin").Select(u => u.Id).ToListAsync();

            if (canHos.Any() && userIds.Any())
            {
                var faker = new Faker("vi");
                var usedPairs = new HashSet<(int, int)>();

                int targetCount = Math.Min(15, canHos.Count * userIds.Count);
                while (usedPairs.Count < targetCount)
                {
                    var canHo = faker.PickRandom(canHos);
                    var userId = faker.PickRandom(userIds);

                    if (usedPairs.Add((canHo.Id, userId)))
                    {
                        canHo.AddQuanHeCuTru(userId, 1, faker.Date.Past(1));
                    }
                }

                context.CanHos.UpdateRange(canHos);
                await context.SaveChangesAsync();
            }
        }
    }
}
