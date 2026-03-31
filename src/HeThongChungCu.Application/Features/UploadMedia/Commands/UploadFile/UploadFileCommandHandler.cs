using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, List<UploadFileResponse>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileCommandHandler(
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions,
        IDateTimeProvider dateTimeProvider,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<UploadFileResponse>>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
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

            var validContainers = new List<string>
            {
                _fileStorageOptions.VehicleContainer,
                _fileStorageOptions.ApartmentContainer,
                _fileStorageOptions.BuildingContainer,
                _fileStorageOptions.UserAvatarContainer,
                _fileStorageOptions.DocumentContainer
            };

            var containerName = validContainers.FirstOrDefault(c =>
                c.Equals(request.TargetContainer, StringComparison.OrdinalIgnoreCase))
                ?? _fileStorageOptions.DocumentContainer;

            var fileUrl = await _fileStorageService.UploadFileAsync(
                file.Content,
                uniqueFileName,
                containerName,
                file.ContentType,
                cancellationToken);

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
