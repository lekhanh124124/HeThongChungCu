using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandHandler : ICommandHandler<TaoMaDinhDanhCommand, string>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IQuanHeCuTruCommandRepository _cuTruRepository;
    private readonly IIdentityDomainService _identityService;
    private readonly IHasherService _hasherService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public TaoMaDinhDanhCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        INguoiDungCommandRepository userRepository,
        IQuanHeCuTruCommandRepository cuTruRepository,
        IIdentityDomainService identityService,
        IHasherService hasherService,
        IDateTimeProvider dateTimeProvider,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _cuTruRepository = cuTruRepository;
        _identityService = identityService;
        _hasherService = hasherService;
        _dateTimeProvider = dateTimeProvider;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(TaoMaDinhDanhCommand request, CancellationToken cancellationToken)
    {
        var cuTru = await _cuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (cuTru is null)
            return Result.Failure<string>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var user = await _userRepository.GetByIdAsync(cuTru.NguoiDungId, cancellationToken);
        if (user is null)
            return Result.Failure<string>(UserErrors.NotFoundById(cuTru.NguoiDungId));

        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (account is null)
            return Result.Failure<string>(AuthErrors.AccountNotFound);

        // Kiểm tra điều kiện định danh
        var isResidentAlreadyLinked = await _accountRepository.AnyAsync(a => a.NguoiDungId == user.Id && a.Id != account.Id, cancellationToken);
        var canLinkResult = _identityService.CanLinkAccountToResident(account, user.Id, isResidentAlreadyLinked);
        if (canLinkResult.IsFailure)
            return Result.Failure<string>(canLinkResult.Errors[0]);

        // Generate JWT token encoding the UserId and AccountId
        var roles = account.PhanQuyens.Select(pq => pq.RoleId.Name).ToList();
        var token = _tokenService.GenerateToken(account.Id, account.TenDangNhap, roles, user.Id);

        // Extract JWT ID (jti) and save it to the account's tokens
        var jti = _tokenService.GetJwtIdFromToken(token);
        account.AddUserCodeToken(jti!, _dateTimeProvider.UtcNow.AddDays(1));

        // Construct the identification link
        var encodedToken = System.Net.WebUtility.UrlEncode(token);
        var identificationLink = $"https://chungcu-webapi-fwf7cva4c7c6ajae.eastasia-01.azurewebsites.net/api/quan-he-cu-tru/xac-nhan-dinh-danh?token={encodedToken}";

        // Send the email
        await _emailService.SendIdentificationEmailAsync(account.Email, identificationLink, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Yêu cầu định danh đã được gửi đến email của người dùng thành công.");
    }
}
