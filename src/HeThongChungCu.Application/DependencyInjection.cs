using HeThongChungCu.Application.Common.Behaviors;
using HeThongChungCu.Domain.DomainServices;
using HeThongChungCu.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        // Domain Services
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IVehicleRegistryService, VehicleRegistryService>();
        services.AddScoped<ICanHoDomainService, CanHoDomainService>();
        services.AddScoped<IResidencyService, ResidencyService>();
        services.AddScoped<IIdentityDomainService, IdentityDomainService>();
        services.AddScoped<IDocumentReconciliationService, DocumentReconciliationService>();
        services.AddScoped<IDichVuDomainService, DichVuDomainService>();

        return services;
    }
}
