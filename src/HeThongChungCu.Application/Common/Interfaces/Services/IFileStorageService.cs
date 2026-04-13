using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, FileCategory category, string contentType, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> UploadFilesAsync(List<(Stream Stream, string FileName, string ContentType)> files, FileCategory category, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, FileCategory? category, CancellationToken cancellationToken = default);
    string UrlNormalization(string fileName, DateTime timestamp);
}
