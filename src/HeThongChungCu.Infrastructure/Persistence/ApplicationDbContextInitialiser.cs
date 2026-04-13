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
            _logger.LogInformation("Testing database connection...");

            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                _logger.LogWarning("Cannot connect to the database. Did you forget to update the database? Run: 'dotnet ef database update'");
            }
            else
            {
                _logger.LogInformation("Database connection successful.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect or verify the database. Application will continue but might fail at runtime.");
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
