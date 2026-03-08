using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<EFDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(EFDbContext).Assembly.FullName)));

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


