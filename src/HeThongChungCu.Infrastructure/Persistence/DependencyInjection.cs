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
        services.Configure<PersistenceOptions>(
            configuration.GetSection(PersistenceOptions.SectionName));

        services.AddDbContext<EFDbContext>((provider, options) =>
        {
            var persistenceOptions = provider
                .GetRequiredService<IOptions<PersistenceOptions>>()
                .Value;

            options.UseSqlServer(
                persistenceOptions.DefaultConnection,
                b => b.MigrationsAssembly(typeof(EFDbContext).Assembly.FullName));
        });

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<DapperDbContext>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EFDbContext>());
        services.AddScoped<IUserEFRepository, UserEFRepository>();
        services.AddScoped<IToaNhaEFRepository, ToaNhaEFRepository>();
        services.AddScoped<IToaNhaDapperRepository, ToaNhaDapperRepository>();
        services.AddScoped<ICanHoEFRepository, CanHoEFRepository>();
        services.AddScoped<ICanHoDapperRepository, CanHoDapperRepository>();
        services.AddScoped<IQuanHeCuTruDapperRepository, QuanHeCuTruDapperRepository>();
        services.AddScoped<IUserDapperRepository, UserDapperRepository>();

        return services;
    }
}


