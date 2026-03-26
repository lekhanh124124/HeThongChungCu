using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    AppDbContext context)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly AppDbContext _context = context;

    public async Task InitialiseAsync()
    {
        if (!_context.Database.IsSqlServer())
            return;

        try
        {
            _logger.LogInformation("Starting database migration...");

            await _context.Database.MigrateAsync();

            _logger.LogInformation("Database migration completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed. Application will continue to start.");
        }
    }

    public void Seed()
    {
        try
        {
            TrySeed();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private void TrySeed()
    {
        // System wide seedings can be put here if necessary (roles, permissions)
        _logger.LogInformation("Initial Seed Check completed.");
    }
}
