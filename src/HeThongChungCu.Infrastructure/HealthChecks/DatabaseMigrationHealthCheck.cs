using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HeThongChungCu.Infrastructure.HealthChecks;

internal sealed class DatabaseMigrationHealthCheck : IHealthCheck
{
    private readonly EFDbContext _dbContext;

    public DatabaseMigrationHealthCheck(EFDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingMigrations = await _dbContext.Database
                .GetPendingMigrationsAsync(cancellationToken);

            var pending = pendingMigrations.ToList();

            if (pending.Any())
            {
                return HealthCheckResult.Degraded(
                    $"Pending migrations: {string.Join(", ", pending)}",
                    data: new Dictionary<string, object>
                    {
                        { "pending_count", pending.Count },
                        { "migrations", pending }
                    });
            }

            return HealthCheckResult.Healthy("All migrations applied");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Failed to check migrations",
                exception: ex);
        }
    }
}
