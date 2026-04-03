using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddHostedService<CleanupUnusedFilesService>();

        return services;
    }
}
