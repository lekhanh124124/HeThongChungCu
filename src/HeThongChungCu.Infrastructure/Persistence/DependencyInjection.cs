using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Infrastructure.Persistence.Interceptors;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;
using HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;
using HeThongChungCu.Infrastructure.Persistence.Seed;
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
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<INguoiDungEFRepository, NguoiDungEFRepository>();
        services.AddScoped<INguoiDungDapperRepository, NguoiDungDapperRepository>();

        services.AddScoped<ITaiKhoanEFRepository, TaiKhoanEFRepository>();

        services.AddScoped<IToaNhaEFRepository, ToaNhaEFRepository>();
        services.AddScoped<IToaNhaDapperRepository, ToaNhaDapperRepository>();

        services.AddScoped<ICanHoEFRepository, CanHoEFRepository>();
        services.AddScoped<ICanHoDapperRepository, CanHoDapperRepository>();

        services.AddScoped<IQuanHeCuTruEFRepository, QuanHeCuTruEFRepository>();
        services.AddScoped<IQuanHeCuTruDapperRepository, QuanHeCuTruDapperRepository>();

        services.AddScoped<IPhuongTienEFRepository, PhuongTienEFRepository>();
        services.AddScoped<IPhuongTienDapperRepository, PhuongTienDapperRepository>();

        services.AddScoped<IYeuCauCuTruEFRepository, YeuCauCuTruEFRepository>();
        services.AddScoped<IYeuCauCuTruDapperRepository, YeuCauCuTruDapperRepository>();

        services.AddScoped<IYeuCauPhuongTienEFRepository, YeuCauPhuongTienEFRepository>();

        services.AddScoped<IDichVuEFRepository, DichVuEFRepository>();
        // services.AddScoped<IDichVuDapperRepository, DichVuDapperRepository>();

        services.AddScoped<ITepTaiLieuRepository, TepTaiLieuRepository>();
        // services.AddScoped<ITepTaiLieuDapperRepository, TepTaiLieuDapperRepository>();

        services.AddScoped<IBangGiaEFRepository, BangGiaEFRepository>();
        // services.AddScoped<IBangGiaDapperRepository, BangGiaDapperRepository>();

        services.AddScoped<IChiSoTieuThuEFRepository, ChiSoTieuThuEFRepository>();
        // services.AddScoped<IChiSoTieuThuDapperRepository, ChiSoTieuThuDapperRepository>();

        services.AddScoped<IDangKyDichVuEFRepository, DangKyDichVuEFRepository>();
        // services.AddScoped<IDangKyDichVuDapperRepository, DangKyDichVuDapperRepository>();
        return services;
    }
}

