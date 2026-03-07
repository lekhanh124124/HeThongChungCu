using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

namespace HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, string>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ResetPasswordCommandHandler(
        IUserEFRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        // TÃ¬m token há»£p lá»‡
        var token = user.Tokens.FirstOrDefault(t =>
            t.TokenType == TokenType.ResetPasswordCode &&
            t.RefreshToken == request.ResetCode &&
            t.IsActive);

        if (token is null)
        {
            return Result.Failure<string>(AuthErrors.InvalidResetToken);
        }

        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(hashedPassword);

        token.Revoke(_dateTimeProvider.UtcNow, ReasonRevoked.ReplacedByNewToken);

        return Result.Success("Đổi mật khẩu thành công.");
    }
}
