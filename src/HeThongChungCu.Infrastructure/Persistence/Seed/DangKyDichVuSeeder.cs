using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DangKyDichVuSeeder
{
    public static Task SeedAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Skipping mandatory service registration seeding. These are now handled implicitly by the Billing Engine.");
        return Task.CompletedTask;
    }
}
