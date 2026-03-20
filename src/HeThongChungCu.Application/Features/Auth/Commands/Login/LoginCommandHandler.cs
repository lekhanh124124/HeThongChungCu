using HeThongChungCu.Application.Features.Auth.DTOs;
using System.Security.Cryptography;

namespace HeThongChungCu.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IUserEFRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find User
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);
        }

        // Check Password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);
        }

        // Get Roles
        Role role = user.RoleId;

        var roles = new List<string> { role.Name };

        var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, roles);
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.AddRefreshToken(refreshTokenString, _dateTimeProvider.UtcNow.AddDays(7));

        return Result.Success(new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AnhDaiDienUrl = user.AnhDaiDienUrl ?? string.Empty,
            Role = role.Name,
            FullName = $"{user.LastName} {user.FirstName}",
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        });
    }
}
