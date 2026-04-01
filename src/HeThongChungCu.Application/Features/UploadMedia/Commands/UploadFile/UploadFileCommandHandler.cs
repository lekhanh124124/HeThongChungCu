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
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileCommandHandler(
        IFileStorageService fileStorageService,
        IDateTimeProvider dateTimeProvider,
        ITepTaiLieuRepository tepTaiLieuRepository,
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
            return Result.Failure<List<UploadFileResponse>>(FileErrors.UnrecognizedCategory);
        }

        var responses = new List<UploadFileResponse>();

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

            var uploadResult = await _fileStorageService.UploadFileAsync(
                file.Content,
                uniqueFileName,
                category,
                file.ContentType,
                cancellationToken);

            if (uploadResult.IsFailure)
            {
                return Result.Failure<List<UploadFileResponse>>(uploadResult.Errors);
            }

            var fileUrl = uploadResult.Value;

            var tepTaiLieu = new TepTaiLieu(file.FileName, fileUrl, file.Content.Length, file.ContentType);
            await _tepTaiLieuRepository.AddAsync(tepTaiLieu, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            responses.Add(new UploadFileResponse(
                tepTaiLieu.Id,
                file.FileName,
                fileUrl,
                file.ContentType));
        }

        return Result.Success(responses);
    }
}
