using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Infrastructure.FileStorage;

public partial class FileStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public FileStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
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

    public async Task DeleteFileAsync(string fileUrl, string? containerName, CancellationToken cancellationToken = default)
    {
        var blobUriBuilder = new BlobUriBuilder(new Uri(fileUrl));
        var effectiveContainerName = string.IsNullOrEmpty(containerName) ? blobUriBuilder.BlobContainerName : containerName;
        var containerClient = _blobServiceClient.GetBlobContainerClient(effectiveContainerName);

        var blobName = blobUriBuilder.BlobName;

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
    }

    public string UrlNormalization(string fileName, DateTime timestamp)
    {
        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        // NormalizedName: lowercase, replace spaces/special chars with hyphens, allow letters, numbers, hyphens, underscores, dots
        // Allowed chars: a-z, 0-9, ., _, -
        var normalizedName = nameWithoutExtension.ToLower().Trim();
        normalizedName = MyRegex().Replace(normalizedName, "-");
        normalizedName = MyRegex1().Replace(normalizedName, "-");

        // Format: 240927-153045
        var timeStr = timestamp.ToString("yyMMdd-HHmmss");

        return $"{normalizedName}-{timeStr}{extension}";
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"[^a-z0-9._-]")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"-+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}
