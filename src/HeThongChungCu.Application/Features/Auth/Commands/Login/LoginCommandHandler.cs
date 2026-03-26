using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using System.Security.Cryptography;

namespace HeThongChungCu.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        ITaiKhoanEFRepository accountRepository,
        INguoiDungEFRepository userRepository,
        IHasherService hasherService,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _hasherService = hasherService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find Account
        var account = await _accountRepository.GetByTenDangNhapAsync(request.Username, cancellationToken);

        if (account is null || !account.IsActive)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);
        }

        // Check Password
        if (!_hasherService.VerifyPassword(request.Password, account.MatKhauHash))
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);
        }

        // Get User details
        var user = account.NguoiDungId.HasValue 
            ? await _userRepository.GetByIdAsync(account.NguoiDungId.Value, cancellationToken)
            : null;

        // Get Roles
        var roles = account.PhanQuyens.Select(pq => pq.RoleId.Name).ToList();

        var accessToken = _jwtTokenGenerator.GenerateToken(account.Id, account.TenDangNhap, roles, account.NguoiDungId);
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenHash = _hasherService.HashToken(refreshTokenString);
 
        account.AddRefreshToken(refreshTokenHash, _dateTimeProvider.UtcNow.AddDays(7));
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse
        {
            UserId = user?.Id,
            AccountId = account.Id,
            Username = account.TenDangNhap,
            Email = account.Email,
            AnhDaiDienUrl = account.AnhDaiDien?.FileUrl ?? string.Empty,
            Role = string.Join(", ", roles),
            FullName = user != null ? $"{user.Ho} {user.Ten}" : "Khách",
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        });
    }
}
