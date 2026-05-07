using HeThongChungCu.Application.Features.QLThanhToan.Commands.QuetHoaDonQuaHan;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class OverdueInvoicesBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OverdueInvoicesBackgroundService> _logger;

    public OverdueInvoicesBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OverdueInvoicesBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverdueInvoicesBackgroundService is starting...");

        // Delay briefly after startup to let the application stabilize
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking and transitioning invoices to 'Quá hạn'...");
                await ScanAndMarkOverdueInvoicesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during overdue invoices scan.");
            }

            // Run once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("OverdueInvoicesBackgroundService is stopping.");
    }

    private async Task ScanAndMarkOverdueInvoicesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await mediator.Send(new QuetHoaDonQuaHanCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            if (result.Value > 0)
            {
                _logger.LogInformation("Successfully updated {Count} overdue invoices via CQRS.", result.Value);
            }
            else
            {
                _logger.LogInformation("Overdue invoices check completed. No new overdue invoices found.");
            }
        }
        else
        {
            _logger.LogWarning("Failed to run overdue invoices scan via CQRS: {Error}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
