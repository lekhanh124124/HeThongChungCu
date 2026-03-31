using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public record UploadFileCommand(
    List<FileUploadItem> Files,
    string? TargetContainer = null) : ICommand<List<UploadFileResponse>>;
