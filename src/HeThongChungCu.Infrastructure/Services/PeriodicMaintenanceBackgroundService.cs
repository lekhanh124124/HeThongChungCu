using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.QuetLichBaoTriVaSinhPhieu;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class PeriodicMaintenanceBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PeriodicMaintenanceBackgroundService> _logger;

    public PeriodicMaintenanceBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PeriodicMaintenanceBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PeriodicMaintenanceBackgroundService is starting...");

        // Delay briefly after startup to let the application stabilize
        await Task.Delay(TimeSpan.FromSeconds(35), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Scanning active maintenance schedules to auto-generate tickets...");
                await AutoGenerateMaintenanceTicketsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during periodic maintenance scheduling scan.");
            }

            // Run once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("PeriodicMaintenanceBackgroundService is stopping.");
    }

    private async Task AutoGenerateMaintenanceTicketsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await mediator.Send(new QuetLichBaoTriVaSinhPhieuCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            if (result.Value > 0)
            {
                _logger.LogInformation("Successfully auto-generated {Count} periodic maintenance tickets via CQRS.", result.Value);
            }
            else
            {
                _logger.LogInformation("Periodic maintenance scheduling completed. No new tickets needed today.");
            }
        }
        else
        {
            _logger.LogWarning("Failed to run periodic maintenance scheduling scan via CQRS: {Error}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
