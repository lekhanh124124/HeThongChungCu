using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        ITaiKhoanEFRepository accountRepository,
        INguoiDungEFRepository userRepository,
        IHasherService hasherService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _hasherService = hasherService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = _hasherService.HashToken(request.RefreshToken);
        var account = await _accountRepository.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (account is null)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var existingToken = account.Tokens.FirstOrDefault(rt => rt.TokenHash == refreshTokenHash);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var user = account.NguoiDungId.HasValue
            ? await _userRepository.GetByIdAsync(account.NguoiDungId.Value, cancellationToken)
            : null;

        if (account.NguoiDungId.HasValue && user is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.NotFound);
        }

        var roles = account.PhanQuyens.Select(pq => pq.RoleId.Name).ToList();
        var newAccessToken = _jwtTokenGenerator.GenerateToken(account.Id, account.TenDangNhap, roles, account.NguoiDungId);

        return Result.Success(new AuthResponse
        {
            UserId = user?.Id,
            AccountId = account.Id,
            Username = account.TenDangNhap,
            Email = account.Email,
            FullName = user != null ? $"{user.Ho} {user.Ten}" : "Khách",
            AnhDaiDienUrl = account.AnhDaiDien?.FileUrl ?? string.Empty,
            Role = string.Join(", ", roles),
            AccessToken = newAccessToken,
            RefreshToken = request.RefreshToken
        });
    }
}
