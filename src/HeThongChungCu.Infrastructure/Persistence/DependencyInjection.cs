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

        services.AddScoped<ITepTaiLieuCommandRepository, TepTaiLieuCommandRepository>();
        services.AddScoped<ITepTaiLieuQueryRepository, TepTaiLieuQueryRepository>();

        services.AddScoped<IThongBaoCommandRepository, ThongBaoCommandRepository>();
        services.AddScoped<IThongBaoQueryRepository, ThongBaoQueryRepository>();

        services.AddScoped<INhanVienCommandRepository, NhanVienCommandRepository>();
        services.AddScoped<INhanVienQueryRepository, NhanVienQueryRepository>();

        services.AddScoped<IDoiTacCommandRepository, DoiTacCommandRepository>();
        services.AddScoped<IDoiTacQueryRepository, DoiTacQueryRepository>();

        services.AddScoped<IHoaDonDoiTacCommandRepository, HoaDonDoiTacCommandRepository>();
        services.AddScoped<IHoaDonDoiTacQueryRepository, HoaDonDoiTacQueryRepository>();

        services.AddScoped<IDichVuCommandRepository, DichVuCommandRepository>();
        services.AddScoped<IDichVuQueryRepository, DichVuQueryRepository>();

        services.AddScoped<IDangKyDichVuCommandRepository, DangKyDichVuCommandRepository>();

        services.AddScoped<IYeuCauSuaChuaCommandRepository, YeuCauSuaChuaCommandRepository>();
        services.AddScoped<IYeuCauSuaChuaQueryRepository, YeuCauSuaChuaQueryRepository>();

        services.AddScoped<IYeuCauThiCongQueryRepository, YeuCauThiCongQueryRepository>();
        services.AddScoped<IYeuCauThiCongCommandRepository, YeuCauThiCongCommandRepository>();
        
        services.AddScoped<IHoaDonCommandRepository, HoaDonCommandRepository>();
        services.AddScoped<IHoaDonQueryRepository, HoaDonQueryRepository>();
        services.AddScoped<IGiaoDichThanhToanCommandRepository, GiaoDichThanhToanCommandRepository>();
        services.AddScoped<IGiaoDichThanhToanQueryRepository, GiaoDichThanhToanQueryRepository>();
        services.AddScoped<IQuyThuChiCommandRepository, QuyThuChiCommandRepository>();
        services.AddScoped<IQuyThuChiQueryRepository, QuyThuChiQueryRepository>();
        services.AddScoped<IChiSoTieuThuCommandRepository, ChiSoTieuThuCommandRepository>();
        services.AddScoped<IChiSoTieuThuQueryRepository, ChiSoTieuThuQueryRepository>();
        services.AddScoped<IDotThanhToanCommandRepository, DotThanhToanCommandRepository>();
        services.AddScoped<IDotThanhToanQueryRepository, DotThanhToanQueryRepository>();
        services.AddScoped<IPhienThanhToanCommandRepository, PhienThanhToanCommandRepository>();
        
        // Phase 4 - Bảo trì hạ tầng
        services.AddScoped<IThietBiCommandRepository, ThietBiCommandRepository>();
        services.AddScoped<IThietBiQueryRepository, ThietBiQueryRepository>();
        services.AddScoped<ILichBaoTriQueryRepository, LichBaoTriQueryRepository>();
        services.AddScoped<IHangMucBaoTriQueryRepository, HangMucBaoTriQueryRepository>();
        services.AddScoped<IPhieuBaoTriCommandRepository, PhieuBaoTriCommandRepository>();
        services.AddScoped<IPhieuBaoTriQueryRepository, PhieuBaoTriQueryRepository>();
        
        // Phase 6 - Phản hồi & Khảo sát ý kiến cư dân
        services.AddScoped<IYeuCauPhanAnhCommandRepository, YeuCauPhanAnhCommandRepository>();
        services.AddScoped<IYeuCauPhanAnhQueryRepository, YeuCauPhanAnhQueryRepository>();
        services.AddScoped<IKhaoSatCommandRepository, KhaoSatCommandRepository>();
        services.AddScoped<IKhaoSatQueryRepository, KhaoSatQueryRepository>();
        services.AddScoped<IBieuQuyetCuDanCommandRepository, BieuQuyetCuDanCommandRepository>();
        services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();

        // AI - Chatbot
        services.AddScoped<ITriThucChatbotCommandRepository, TriThucChatbotCommandRepository>();
        services.AddScoped<ITriThucChatbotQueryRepository, TriThucChatbotQueryRepository>();
        
        return services;
    }
}

