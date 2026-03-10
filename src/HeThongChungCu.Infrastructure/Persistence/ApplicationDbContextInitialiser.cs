using Bogus;
using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly EFDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, EFDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        await HeThongChungCu.Infrastructure.Persistence.Seed.UserSeeder.SeedAsync(_context, _logger);
        await HeThongChungCu.Infrastructure.Persistence.Seed.ToaNhaSeeder.SeedAsync(_context, _logger);
        await HeThongChungCu.Infrastructure.Persistence.Seed.CanHoSeeder.SeedAsync(_context, _logger);
        await HeThongChungCu.Infrastructure.Persistence.Seed.QuanHeCuTruSeeder.SeedAsync(_context, _logger);
        await HeThongChungCu.Infrastructure.Persistence.Seed.PhuongTienSeeder.SeedAsync(_context, _logger);

        _logger.LogInformation("Database Seeding Completed.");
    }
}
