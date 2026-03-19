using Bogus;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class QuanHeCuTruSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int targetCount)
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
                var apartmentsWithChuHo = new HashSet<int>();
                var otherRoles = new[] { LoaiQuanHeCuTru.NguoiThue, LoaiQuanHeCuTru.NguoiOCung, LoaiQuanHeCuTru.Khac };

                // A user can now belong to multiple apartments, so the max count is based on unique (Apartment, User) pairs
                targetCount = Math.Min(targetCount, canHos.Count * userIds.Count);
                var generatedRelations = new List<QuanHeCuTru>();

                while (generatedRelations.Count < targetCount)
                {
                    var canHo = faker.PickRandom(canHos);
                    var userId = faker.PickRandom(userIds);

                    if (usedPairs.Add((canHo.Id, userId)))
                    {
                        var loaiQuanHe = LoaiQuanHeCuTru.ChuHo;
                        
                        if (apartmentsWithChuHo.Contains(canHo.Id))
                        {
                            loaiQuanHe = faker.PickRandom(otherRoles);
                        }
                        else
                        {
                            apartmentsWithChuHo.Add(canHo.Id);
                        }

                        var quanHe = new QuanHeCuTru(
                            canHo.Id, 
                            userId, 
                            loaiQuanHe, 
                            faker.Date.Past(1), 
                            generatedRelations.Where(r => r.CanHoId == canHo.Id));
                        
                        generatedRelations.Add(quanHe);
                        context.QuanHeCuTrus.Add(quanHe);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
