using Microsoft.Extensions.Logging;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using MediatR;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.CleanupUnusedFiles;

public class CleanupUnusedFilesCommandHandler : ICommandHandler<CleanupUnusedFilesCommand, int>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CleanupUnusedFilesCommandHandler> _logger;

    public CleanupUnusedFilesCommandHandler(
        IDateTimeProvider dateTimeProvider,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<CleanupUnusedFilesCommandHandler> _logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        this._logger = _logger;
    }

    public async Task<Result<int>> Handle(CleanupUnusedFilesCommand request, CancellationToken cancellationToken)
    {
        var before = _dateTimeProvider.UtcNow.DateTime.AddHours(-request.ThresholdHours);
        _logger.LogInformation("CQRS: Scanning for unused files created before {ThresholdTime}", before);

        var unusedFiles = (await _tepTaiLieuRepository.GetUnusedFilesAsync(before, cancellationToken)).ToList();

        if (unusedFiles.Count == 0)
        {
            return 0;
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

        return unusedFiles.Count;
    }
}
