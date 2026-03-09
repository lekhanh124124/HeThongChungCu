namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, string containerName, CancellationToken cancellationToken = default);
    string UrlNormalization(string fileName, DateTime timestamp);
}