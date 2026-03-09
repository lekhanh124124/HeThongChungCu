using HeThongChungCu.Infrastructure.Authentication;
using HeThongChungCu.Infrastructure.Email;
using HeThongChungCu.Infrastructure.FileStorage;
using HeThongChungCu.Infrastructure.HealthChecks;
using HeThongChungCu.Infrastructure.Persistence;
using HeThongChungCu.Infrastructure.Qdrant;
using HeThongChungCu.Infrastructure.Services;

namespace HeThongChungCu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddServices();
        services.AddAuthLayer(configuration);
        services.AddHealthChecks(configuration);
        services.AddEmail(configuration);
        services.AddQdrantVectorStore(configuration);
        services.AddFileStorage(configuration);

        return services;
    }
}
