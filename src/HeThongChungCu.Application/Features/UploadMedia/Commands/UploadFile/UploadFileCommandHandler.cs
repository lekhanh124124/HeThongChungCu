using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, List<UploadFileResponse>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileCommandHandler(
        IFileStorageService fileStorageService,
        IDateTimeProvider dateTimeProvider,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _fileStorageService = fileStorageService;
        _dateTimeProvider = dateTimeProvider;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<UploadFileResponse>>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var category = FileCategory.FromTargetContainer(request.TargetContainer ?? string.Empty);
        if (category == null)
        {
            return FileErrors.UnrecognizedCategory;
        }

        var uploadData = new List<(Stream Stream, string FileName, string ContentType)>();
        var originalFileNameMap = new Dictionary<string, string>(); // UniqueName -> OriginalName
        var contentTypeMap = new Dictionary<string, string>(); // UniqueName -> ContentType

        foreach (var file in request.Files)
        {
            if (file.Content.CanSeek)
            {
                file.Content.Position = 0;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = _fileStorageService.UrlNormalization(
                $"{Guid.NewGuid():N}{extension}",
                _dateTimeProvider.UtcNow.DateTime);

            uploadData.Add((file.Content, uniqueFileName, file.ContentType));
            originalFileNameMap[uniqueFileName] = file.FileName;
            contentTypeMap[uniqueFileName] = file.ContentType;
        }

        var uploadResult = await _fileStorageService.UploadFilesAsync(
            uploadData,
            category,
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return uploadResult.Errors;
        }

        var fileUrls = uploadResult.Value;
        var tepTaiLieus = new List<TepTaiLieu>();
        var responses = new List<UploadFileResponse>();

        for (int i = 0; i < fileUrls.Count; i++)
        {
            var url = fileUrls[i];
            var uniqueName = uploadData[i].FileName;
            var originalName = originalFileNameMap[uniqueName];
            var contentType = contentTypeMap[uniqueName];
            var streamSize = uploadData[i].Stream.Length;

            var tepTaiLieu = new TepTaiLieu(originalName, url, streamSize, contentType);
            tepTaiLieus.Add(tepTaiLieu);
        }

        await _tepTaiLieuRepository.AddRangeAsync(tepTaiLieus, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        for (int i = 0; i < tepTaiLieus.Count; i++)
        {
            var entity = tepTaiLieus[i];
            responses.Add(new UploadFileResponse(
                entity.Id,
                entity.FileName,
                entity.FileUrl,
                entity.ContentType));
        }

        return responses;
    }
}
