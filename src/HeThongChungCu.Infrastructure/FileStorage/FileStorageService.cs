using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.FileStorage;

public class FileStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly FileStorageOptions _options;

    public FileStorageService(BlobServiceClient blobServiceClient, IOptions<FileStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.Blob,
            cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileUrl, string containerName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobUriBuilder = new BlobUriBuilder(new Uri(fileUrl));
        var blobName = blobUriBuilder.BlobName;

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
    }

    public string UrlNormalization(string fileName, DateTime timestamp)
    {
        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        // Chuẩn hóa tên: lower, thay khoảng trắng/ký tự lạ bằng dấu gạch ngang
        // Cho phép chữ cái, số, gạch ngang, gạch dưới, dấu chấm
        var normalizedName = nameWithoutExtension.ToLower().Trim();
        normalizedName = System.Text.RegularExpressions.Regex.Replace(normalizedName, @"[^a-z0-9._-]", "-");
        normalizedName = System.Text.RegularExpressions.Regex.Replace(normalizedName, @"-+", "-");

        // Định dạng thời gian: yymmdd-HHmmss
        var timeStr = timestamp.ToString("yyMMdd-HHmmss");

        return $"{normalizedName}-{timeStr}{extension}";
    }
}
