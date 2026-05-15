using HeThongChungCu.Application.Features.QLPhanAnh.Commands.QuetPhanAnhQuaHan;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class OverduePhanAnhBackgroundService : BackgroundService
{
    private readonly ILogger<OverduePhanAnhBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OverduePhanAnhBackgroundService(
        ILogger<OverduePhanAnhBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverduePhanAnhBackgroundService is starting.");

        // Chạy mỗi 30 phút
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("OverduePhanAnhBackgroundService is running.");

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                try
                {
                    var command = new QuetPhanAnhQuaHanCommand();
                    var result = await mediator.Send(command, stoppingToken);

                    if (result.IsFailure)
                    {
                        _logger.LogWarning("OverduePhanAnhBackgroundService encountered an error: {Error}", result.Errors.FirstOrDefault()?.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing QuetPhanAnhQuaHanCommand.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OverduePhanAnhBackgroundService is stopping.");
        }
    }
}
