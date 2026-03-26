namespace HeThongChungCu.Application.Features.UploadMedia.DTOs;

public record UploadFileResponse(
    int FileId,
    string FileName,
    string FileUrl,
    string ContentType);
