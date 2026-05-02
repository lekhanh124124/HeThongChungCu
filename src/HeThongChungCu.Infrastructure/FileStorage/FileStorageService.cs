using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.FileStorage;

public partial class FileStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageSettings _settings;

    // File signatures (Magic Numbers) - Used for deep validation of file content
    private static readonly byte[] _jpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] _pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] _pdfSignature = [0x25, 0x50, 0x44, 0x46]; // Hex for "%PDF"

    private static readonly Dictionary<string, byte[]> _fileSignatures = new()
    {
        { ".jpg", _jpegSignature },
        { ".jpeg", _jpegSignature },
        { ".png", _pngSignature },
        { ".pdf", _pdfSignature }
    };

    public FileStorageService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageSettings> settings)
    {
        _blobServiceClient = blobServiceClient;
        _settings = settings.Value;
    }

    public async Task<Result<string>> UploadFileAsync(
        Stream fileStream,
        string fileName,
        FileCategory category,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        // 1. Validation (Internalized logic)
        var validationResult = Validate(fileStream, fileName, category);
        if (validationResult.IsFailure)
        {
            return Result.Failure<string>(validationResult.Errors);
        }

        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var containerName = GetContainerName(category);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.Blob,
            cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return Result.Success(blobClient.Uri.ToString());
    }

    public async Task<Result<List<string>>> UploadFilesAsync(
        List<(Stream Stream, string FileName, string ContentType)> files,
        FileCategory category,
        CancellationToken cancellationToken = default)
    {
        var urls = new List<string>();
        var uploadTasks = new List<Task<Result<string>>>();

        foreach (var file in files)
        {
            uploadTasks.Add(UploadFileAsync(file.Stream, file.FileName, category, file.ContentType, cancellationToken));
        }

        var results = await Task.WhenAll(uploadTasks);

        var errors = results.Where(r => r.IsFailure).SelectMany(r => r.Errors).ToList();
        if (errors.Count > 0)
        {
            return Result.Failure<List<string>>(errors);
        }

        urls.AddRange(results.Select(r => r.Value));
        return Result.Success(urls);
    }

    public async Task DeleteFileAsync(string fileUrl, FileCategory? category, CancellationToken cancellationToken = default)
    {
        var blobUriBuilder = new BlobUriBuilder(new Uri(fileUrl));
        var containerName = category != null ? GetContainerName(category) : blobUriBuilder.BlobContainerName;

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobName = blobUriBuilder.BlobName;

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
    }

    private static Result Validate(Stream stream, string fileName, FileCategory category)
    {
        // 1. Check Extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!category.AllowedExtensions.Contains(extension))
        {
            return Result.Failure(FileErrors.InvalidType(extension, category.AllowedExtensions));
        }

        // 2. Check Size
        var sizeInMB = stream.Length / (1024.0 * 1024.0);
        if (sizeInMB > category.MaxSizeMB)
        {
            return Result.Failure(FileErrors.TooLarge(category.MaxSizeMB));
        }

        // 3. Check Signature (Magic Numbers) for security
        if (_fileSignatures.TryGetValue(extension, out var expectedSignature))
        {
            if (stream.CanSeek)
            {
                var originalPosition = stream.Position;
                stream.Position = 0;

                try
                {
                    using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
                    var headerBytes = reader.ReadBytes(expectedSignature.Length);

                    if (!headerBytes.SequenceEqual(expectedSignature))
                    {
                        return Result.Failure(FileErrors.SignatureMismatch);
                    }
                }
                finally
                {
                    stream.Position = originalPosition;
                }
            }
        }

        return Result.Success();
    }

    private string GetContainerName(FileCategory category)
    {
        if (category == FileCategory.Avatar) return _settings.UserAvatarContainer;
        if (category == FileCategory.Building) return _settings.BuildingContainer;
        if (category == FileCategory.Apartment) return _settings.ApartmentContainer;
        if (category == FileCategory.Document) return _settings.DocumentContainer;
        if (category == FileCategory.Vehicle) return _settings.VehicleContainer;
        if (category == FileCategory.StaffDocument) return _settings.StaffDocumentContainer;
        if (category == FileCategory.PartnerDocument) return _settings.PartnerDocumentContainer;
        if (category == FileCategory.MeterReading) return _settings.MeterReadingContainer;

        return _settings.DocumentContainer; // Default
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
