using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandHandler : ICommandHandler<UpdateAvatarCommand, string>
{
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateAvatarCommandHandler(
        INguoiDungEFRepository userRepository,
        ITaiKhoanEFRepository accountRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<string>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.AccountId;
        if (accountId == null)
        {
            return Result.Failure<string>(AuthErrors.InvalidCredentials);
        }

        var account = await _accountRepository.GetWithAvatarAsync(accountId.Value, cancellationToken);
        if (account == null)
        {
            return Result.Failure<string>(AuthErrors.InvalidCredentials);
        }

        var identifier = account.TenDangNhap;

        // 2. Reset stream position (just in case)
        if (request.AvatarStream.CanSeek)
        {
            request.AvatarStream.Position = 0;
        }

        // 3. Upload New Avatar
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        var fileName = $"{identifier}{extension}";

        var normalizedFileName = _fileStorageService.UrlNormalization(
            fileName,
            _dateTimeProvider.UtcNow.DateTime);

        var avatarUrl = await _fileStorageService.UploadFileAsync(
            request.AvatarStream,
            normalizedFileName,
            _fileStorageOptions.UserAvatarContainer,
            request.ContentType,
            cancellationToken);

        // 4. Create TepTaiLieu
        var tepTaiLieu = new TepTaiLieu(request.FileName, avatarUrl, request.AvatarStream.Length, request.ContentType);
        tepTaiLieu.MarkAsUsed();
        await _tepTaiLieuRepository.AddAsync(tepTaiLieu, cancellationToken);

        // 5. Update Database
        var oldAvatar = account.AnhDaiDien;
        oldAvatar?.MarkAsUnused();

        account.UpdateAvatar(tepTaiLieu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return avatarUrl;
    }
}
