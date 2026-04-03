using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Infrastructure.Common.Settings;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using HeThongChungCu.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.Configure<PersistenceSettings>(
            configuration.GetSection(PersistenceSettings.SectionName));

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var persistenceOptions = provider
                .GetRequiredService<IOptions<PersistenceSettings>>()
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
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<AppDbContext>());

        services.AddScoped<INguoiDungCommandRepository, NguoiDungCommandRepository>();
        services.AddScoped<INguoiDungQueryRepository, NguoiDungQueryRepository>();

        services.AddScoped<ITaiKhoanCommandRepository, TaiKhoanCommandRepository>();

        services.AddScoped<IToaNhaCommandRepository, ToaNhaCommandRepository>();
        services.AddScoped<IToaNhaQueryRepository, ToaNhaQueryRepository>();

        services.AddScoped<ICanHoCommandRepository, CanHoCommandRepository>();
        services.AddScoped<ICanHoQueryRepository, CanHoQueryRepository>();

        services.AddScoped<IQuanHeCuTruCommandRepository, QuanHeCuTruCommandRepository>();
        services.AddScoped<IQuanHeCuTruQueryRepository, QuanHeCuTruQueryRepository>();

        services.AddScoped<IPhuongTienCommandRepository, PhuongTienCommandRepository>();
        services.AddScoped<IPhuongTienQueryRepository, PhuongTienQueryRepository>();

        services.AddScoped<IYeuCauCuTruCommandRepository, YeuCauCuTruCommandRepository>();
        services.AddScoped<IYeuCauCuTruQueryRepository, YeuCauCuTruQueryRepository>();

        services.AddScoped<IYeuCauPhuongTienCommandRepository, YeuCauPhuongTienCommandRepository>();
        services.AddScoped<IYeuCauPhuongTienQueryRepository, YeuCauPhuongTienQueryRepository>();

        services.AddScoped<ITepTaiLieuRepository, TepTaiLieuRepository>();
        // services.AddScoped<ITepTaiLieuQueryRepository, TepTaiLieuQueryRepository>();
        
        services.AddScoped<IThongBaoCommandRepository, ThongBaoCommandRepository>();
        services.AddScoped<IThongBaoQueryRepository, ThongBaoQueryRepository>();
        
        services.AddScoped<INhanVienCommandRepository, NhanVienCommandRepository>();
        services.AddScoped<INhanVienQueryRepository, NhanVienQueryRepository>();
        
        return services;
    }
}

