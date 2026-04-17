using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandHandler : ICommandHandler<UpdateAvatarCommand, string>
{
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateAvatarCommandHandler(
        INguoiDungCommandRepository userRepository,
        ITaiKhoanCommandRepository accountRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
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

        // 3. Upload New Avatar
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        var fileName = $"{identifier}{extension}";

        var normalizedFileName = _fileStorageService.UrlNormalization(
            fileName,
            _dateTimeProvider.UtcNow.DateTime);

        var uploadResult = await _fileStorageService.UploadFileAsync(
            request.AvatarStream,
            normalizedFileName,
            FileCategory.Avatar,
            request.ContentType,
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<string>(uploadResult.Errors);
        }

        var avatarUrl = uploadResult.Value;

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
