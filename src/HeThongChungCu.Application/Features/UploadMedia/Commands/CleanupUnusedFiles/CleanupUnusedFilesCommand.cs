namespace HeThongChungCu.Application.Features.UploadMedia.Commands.CleanupUnusedFiles;

public record CleanupUnusedFilesCommand(
    int ThresholdHours) : IRequest<Result<int>>;

