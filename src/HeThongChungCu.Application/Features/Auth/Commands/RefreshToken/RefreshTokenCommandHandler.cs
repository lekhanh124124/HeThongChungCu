using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public RefreshTokenCommandHandler(
        IUserEFRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var existingToken = user.Tokens.FirstOrDefault(rt => rt.RefreshToken == request.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
        }

        var roles = new List<string> { user.RoleId.Name };
        var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, roles);

        return Result.Success(new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            AnhDaiDienUrl = user.AnhDaiDienUrl ?? string.Empty,
            Role = user.RoleId.Name,
            AccessToken = newAccessToken,
            RefreshToken = request.RefreshToken
        });
    }
}
