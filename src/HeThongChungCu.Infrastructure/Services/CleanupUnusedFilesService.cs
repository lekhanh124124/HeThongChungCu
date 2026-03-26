using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.Services;

public class CleanupUnusedFilesService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupUnusedFilesService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public CleanupUnusedFilesService(
        IServiceProvider serviceProvider,
        ILogger<CleanupUnusedFilesService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CleanupUnusedFilesService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during unused files cleanup.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("CleanupUnusedFilesService is stopping.");
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tepTaiLieuRepository = scope.ServiceProvider.GetRequiredService<ITepTaiLieuRepository>();
        var fileStorageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        var fileStorageOptions = scope.ServiceProvider.GetRequiredService<IOptions<FileStorageOptions>>().Value;
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _logger.LogInformation("Scanning for unused files created before {Time}", DateTime.UtcNow.AddHours(-1));

        var before = DateTime.UtcNow.AddHours(-1);
        var unusedFiles = (await tepTaiLieuRepository.GetUnusedFilesAsync(before, cancellationToken)).ToList();

        if (unusedFiles.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {Count} unused files to delete.", unusedFiles.Count);

        foreach (var file in unusedFiles)
        {
            try
            {
                _logger.LogInformation("Deleting unused file from storage: {FileUrl}", file.FileUrl);

                await fileStorageService.DeleteFileAsync(
                    file.FileUrl,
                    null,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete unused file from storage: {FileUrl}", file.FileUrl);
            }
        }

        _logger.LogInformation("Deleting {Count} unused file records from database.", unusedFiles.Count);
        tepTaiLieuRepository.DeleteRange(unusedFiles);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
