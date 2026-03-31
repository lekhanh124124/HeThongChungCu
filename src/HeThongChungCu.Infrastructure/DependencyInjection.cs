using HeThongChungCu.Infrastructure.Authentication;
using HeThongChungCu.Infrastructure.Email;
using HeThongChungCu.Infrastructure.FileStorage;
using HeThongChungCu.Infrastructure.HealthChecks;
using HeThongChungCu.Infrastructure.Persistence;
using HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;
using HeThongChungCu.Infrastructure.Qdrant;
using HeThongChungCu.Infrastructure.Services;
using HeThongChungCu.Infrastructure.Notifications;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddNotification();
        return services;
    }
}
