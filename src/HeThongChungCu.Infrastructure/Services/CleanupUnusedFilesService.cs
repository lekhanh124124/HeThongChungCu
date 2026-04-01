using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.UploadMedia.Commands.CleanupUnusedFiles;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Common.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.Services;

public class CleanupUnusedFilesService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupUnusedFilesService> _logger;
    private readonly FileCleanupSettings _settings;

    public CleanupUnusedFilesService(
        IServiceProvider serviceProvider,
        ILogger<CleanupUnusedFilesService> logger,
        IOptions<FileCleanupSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CleanupUnusedFilesService is starting with interval: {Interval} hours.", _settings.CleanupIntervalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting unused files cleanup process...");
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during unused files cleanup.");
            }

            await Task.Delay(TimeSpan.FromHours(_settings.CleanupIntervalHours), stoppingToken);
        }

        _logger.LogInformation("CleanupUnusedFilesService is stopping.");
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await mediator.Send(new CleanupUnusedFilesCommand(_settings.UnusedFileThresholdHours), cancellationToken);

        if (result.IsSuccess && result.Value > 0)
        {
            _logger.LogInformation("Successfully cleaned up {Count} unused files via CQRS.", result.Value);
        }
    }
}
