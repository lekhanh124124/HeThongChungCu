using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.CleanupUnusedFiles;

public class CleanupUnusedFilesCommandHandler : IRequestHandler<CleanupUnusedFilesCommand, Result<int>>
{
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CleanupUnusedFilesCommandHandler> _logger;

    public CleanupUnusedFilesCommandHandler(
        ITepTaiLieuRepository tepTaiLieuRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<CleanupUnusedFilesCommandHandler> _logger)
    {
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        this._logger = _logger;
    }

    public async Task<Result<int>> Handle(CleanupUnusedFilesCommand request, CancellationToken cancellationToken)
    {
        var before = DateTime.UtcNow.AddHours(-request.ThresholdHours);
        _logger.LogInformation("CQRS: Scanning for unused files created before {ThresholdTime}", before);

        var unusedFiles = (await _tepTaiLieuRepository.GetUnusedFilesAsync(before, cancellationToken)).ToList();

        if (unusedFiles.Count == 0)
        {
            return Result.Success(0);
        }

        foreach (var file in unusedFiles)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(file.FileUrl, null, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CQRS: Failed to delete file {FileUrl}", file.FileUrl);
            }
        }

        _tepTaiLieuRepository.DeleteRange(unusedFiles);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(unusedFiles.Count);
    }
}
