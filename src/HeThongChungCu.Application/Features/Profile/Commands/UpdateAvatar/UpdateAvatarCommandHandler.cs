using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandHandler : ICommandHandler<UpdateAvatarCommand, string>
{
    private readonly IUserEFRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvatarCommandHandler(
        IUserEFRepository userRepository,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        // 2. Reset stream position (just in case)
        if (request.AvatarStream.CanSeek)
        {
            request.AvatarStream.Position = 0;
        }

        // 3. Upload New Avatar
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        var fileName = $"{user.Username}{extension}";

        var normalizedFileName = _fileStorageService.UrlNormalization(
            fileName,
            DateTime.UtcNow);

        var avatarUrl = await _fileStorageService.UploadFileAsync(
            request.AvatarStream,
            normalizedFileName,
            _fileStorageOptions.UserAvatarContainer,
            request.ContentType,
            cancellationToken);

        // 4. Update Database
        var oldAvatarUrl = user.AnhDaiDienUrl;
        user.UpdateAvatar(avatarUrl);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Delete Old Avatar (if exists and different)
        if (!string.IsNullOrEmpty(oldAvatarUrl) && oldAvatarUrl != avatarUrl)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(oldAvatarUrl, _fileStorageOptions.UserAvatarContainer, cancellationToken);
            }
            catch
            {
                // Log warning but don't fail the request since the new one is already set
            }
        }

        return avatarUrl;
    }
}
