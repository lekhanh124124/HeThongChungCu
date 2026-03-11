using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.Configure<PersistenceOptions>(
            configuration.GetSection(PersistenceOptions.SectionName));

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var persistenceOptions = provider
                .GetRequiredService<IOptions<PersistenceOptions>>()
                .Value;

            options
                .UseSqlServer(
                    persistenceOptions.DefaultConnection,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
        });

        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserEFRepository, UserEFRepository>();
        services.AddScoped<IToaNhaEFRepository, ToaNhaEFRepository>();
        services.AddScoped<IToaNhaDapperRepository, ToaNhaDapperRepository>();
        services.AddScoped<ICanHoEFRepository, CanHoEFRepository>();
        services.AddScoped<ITangEFRepository, TangEFRepository>();
        services.AddScoped<ITangDapperRepository, TangDapperRepository>();
        services.AddScoped<ICanHoDapperRepository, CanHoDapperRepository>();
        services.AddScoped<IQuanHeCuTruDapperRepository, QuanHeCuTruDapperRepository>();
        services.AddScoped<IUserDapperRepository, UserDapperRepository>();

        return services;
    }
}


