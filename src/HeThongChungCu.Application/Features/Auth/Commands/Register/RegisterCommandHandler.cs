using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using System.Security.Cryptography;

namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
{

    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly IHasherService _hasherService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        IHasherService hasherService,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _hasherService = hasherService;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _accountRepository.AnyAsync(a => a.Email.Value == request.Email || a.TenDangNhap == request.Email, cancellationToken);
        if (userExists)
        {
            return UserErrors.EmailAlreadyExists;
        }

        // 1. Create Account (Auth)
        var hashedPassword = _hasherService.HashPassword(request.Password);
        var account = new TaiKhoan(
            null, // NguoiDungId is null initially
            request.Email, // TenDangNhap = Email
            request.Email,
            hashedPassword);

        // Assign default role Guest
        account.AddRole(Role.Guest);

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = account.PhanQuyens.Select(pq => pq.RoleId.Name).ToList();
        var accessToken = _tokenService.GenerateToken(account.Id, account.TenDangNhap, roles, account.NguoiDungId);
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenHash = _hasherService.HashToken(refreshTokenString);
 
        account.AddRefreshToken(refreshTokenHash, _dateTimeProvider.UtcNow.AddMinutes(_tokenService.RefreshTokenExpiryMinutes));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse
        {
            UserId = null,
            AccountId = account.Id,
            Username = account.TenDangNhap,
            Email = account.Email,
            AnhDaiDienUrl = string.Empty,
            Role = string.Join(", ", roles),
            FullName = "Khách",
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        });
    }
}
