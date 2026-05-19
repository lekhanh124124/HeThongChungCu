using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using HeThongChungCu.Application.Features.QLSystem.Commands.ProcessAutoBackup;

namespace HeThongChungCu.Infrastructure.Services;

public class DatabaseBackupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseBackupBackgroundService> _logger;

    public DatabaseBackupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseBackupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DatabaseBackupBackgroundService is starting...");

        // Chờ một khoảng thời gian ngắn (30 giây) sau khi startup để đảm bảo toàn bộ hệ thống đã khởi tạo hoàn tất
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.AddHours(1); // 01:00 AM hôm nay

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); // 01:00 AM ngày mai
            }

            var delay = nextRun - now;
            _logger.LogInformation("DatabaseBackupBackgroundService: Next run scheduled at {NextRunTime} (delaying for {Delay})", nextRun, delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break; // Hủy bỏ nếu ứng dụng đang tắt
            }

            try
            {
                _logger.LogInformation("DatabaseBackupBackgroundService: Triggering automated daily backup...");
                await TriggerAutoBackupAsync(stoppingToken);
                _logger.LogInformation("DatabaseBackupBackgroundService: Automated daily backup completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DatabaseBackupBackgroundService: An error occurred during daily automated database backup.");
            }
        }

        _logger.LogInformation("DatabaseBackupBackgroundService is stopping.");
    }

    private async Task TriggerAutoBackupAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Gửi Command tới tầng Application để xử lý
        var result = await mediator.Send(new ProcessAutoBackupCommand(), cancellationToken);

        if (result.IsFailure)
        {
            throw new Exception($"Backup failed with errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
