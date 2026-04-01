using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandHandler : ICommandHandler<TaoMaDinhDanhCommand, string>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public TaoMaDinhDanhCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        INguoiDungCommandRepository userRepository,
        IHasherService hasherService,
        IDateTimeProvider dateTimeProvider,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _hasherService = hasherService;
        _dateTimeProvider = dateTimeProvider;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(TaoMaDinhDanhCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<string>(UserErrors.NotFoundById(request.UserId));

        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (account is null)
            return Result.Failure<string>(AuthErrors.AccountNotFound);

        // Generate JWT token encoding the UserId and AccountId
        var roles = account.PhanQuyens.Select(pq => pq.RoleId.Name).ToList();
        var token = _tokenService.GenerateToken(account.Id, account.TenDangNhap, roles, user.Id);

        // Hash the token and save it to the account
        var tokenHash = _hasherService.HashToken(token);
        account.AddUserCodeToken(tokenHash, _dateTimeProvider.UtcNow.AddDays(1));

        // Construct the identification link (placeholder URL)
        var identificationLink = $"https://chungcu-webapi-fwf7cva4c7c6ajae.eastasia-01.azurewebsites.net/xac-nhan-dinh-danh?token={token}";

        // Send the email
        await _emailService.SendIdentificationEmailAsync(account.Email, identificationLink, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Yêu cầu định danh đã được gửi đến email của người dùng thành công.");
    }
}
