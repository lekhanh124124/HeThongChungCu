using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Notifications;

namespace HeThongChungCu.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IZipService, ZipService>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
        services.AddHostedService<CleanupUnusedFilesService>();
        services.AddHostedService<MonthlyBillingBackgroundService>();
        services.AddHostedService<NotificationBackgroundService>();
        services.AddHostedService<OverdueInvoicesBackgroundService>();
        services.AddHostedService<PeriodicMaintenanceBackgroundService>();
        services.AddHostedService<CampaignSchedulerBackgroundService>();
        services.AddHostedService<OverduePhanAnhBackgroundService>();
        services.AddHostedService<DatabaseBackupBackgroundService>();

        return services;
    }
}
