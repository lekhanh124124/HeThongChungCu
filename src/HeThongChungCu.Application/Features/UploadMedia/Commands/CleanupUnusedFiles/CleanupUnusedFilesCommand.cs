using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using MediatR;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.CleanupUnusedFiles;

public record CleanupUnusedFilesCommand(
    int ThresholdHours) : ICommand<int>;
