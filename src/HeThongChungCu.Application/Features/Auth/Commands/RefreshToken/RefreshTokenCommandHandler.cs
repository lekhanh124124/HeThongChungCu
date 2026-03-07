using System.Security.Cryptography;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(
        IUserEFRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.Token, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var existingToken = user.Tokens.FirstOrDefault(rt => rt.RefreshToken == request.Token);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var newRefreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        existingToken.Revoke(_dateTimeProvider.UtcNow, ReasonRevoked.ReplacedByNewToken, newRefreshTokenString);

        var roles = new List<string> { Role.FromValue(user.RoleId)!.Name };
        var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, roles);

        var newRefreshToken = Tokens.CreateRefreshToken(user.Id, newRefreshTokenString, _dateTimeProvider.UtcNow.AddDays(7));
        user.AddToken(newRefreshToken);

        return Result.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString
        });
    }
}
