using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.CloseExpiredKhaoSat;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class CampaignSchedulerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CampaignSchedulerBackgroundService> _logger;

    public CampaignSchedulerBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CampaignSchedulerBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CampaignSchedulerBackgroundService is starting...");

        // Delay briefly after startup to let the application stabilize
        await Task.Delay(TimeSpan.FromSeconds(40), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Scanning active surveys and elections to auto-close expired ones...");
                await AutoCloseExpiredCampaignsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during campaign scheduler background scanning.");
            }

            // Run once every 1 hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        _logger.LogInformation("CampaignSchedulerBackgroundService is stopping.");
    }

    private async Task AutoCloseExpiredCampaignsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await mediator.Send(new CloseExpiredKhaoSatCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            if (result.Value > 0)
            {
                _logger.LogInformation("Successfully closed {Count} expired surveys/elections via CQRS.", result.Value);
            }
            else
            {
                _logger.LogInformation("Campaign scheduler scan completed. No expired campaigns found.");
            }
        }
        else
        {
            _logger.LogWarning("Failed to run campaign scheduler scan via CQRS: {Error}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
