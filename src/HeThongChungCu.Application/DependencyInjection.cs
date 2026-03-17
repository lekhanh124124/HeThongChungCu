using HeThongChungCu.Application.Common.Behaviors;
using HeThongChungCu.Domain.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
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

        // Domain Policies
        services.AddScoped<ICuTruPolicy, CuTruPolicy>();
        services.AddScoped<IToaNhaPolicy, ToaNhaPolicy>();
        services.AddScoped<IPhuongTienPolicy, PhuongTienPolicy>();
        services.AddScoped<IChiSoTieuThuPolicy, ChiSoTieuThuPolicy>();
        services.AddScoped<ICanHoPolicy, CanHoPolicy>();

        return services;
    }
}
