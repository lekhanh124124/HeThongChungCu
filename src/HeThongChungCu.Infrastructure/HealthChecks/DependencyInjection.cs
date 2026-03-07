using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HeThongChungCu.Infrastructure.HealthChecks;

public static class DependencyInjection
{
    public static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            // ═══════════════════════════════════════════════════════════════
            // DATABASE
            // ═══════════════════════════════════════════════════════════════
            // SQL Server check
            .AddDbContextCheck<Persistence.EFDbContext>(
                name: "sql-server",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "sqlserver", "ready" })

            // ═══════════════════════════════════════════════════════════════
            // EXTERNAL SERVICES
            // ═══════════════════════════════════════════════════════════════
            // Qdrant Vector DB check
            .AddCheck<QdrantHealthCheck>(
                "qdrant-vector-db",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "external", "vector-db", "qdrant", "ready" })

            // ═══════════════════════════════════════════════════════════════
            // CUSTOM CHECKS
            // ═══════════════════════════════════════════════════════════════
            // Migrations check
            .AddCheck<DatabaseMigrationHealthCheck>(
                "database-migrations",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "migrations", "ready" });

        return services;
    }
}
