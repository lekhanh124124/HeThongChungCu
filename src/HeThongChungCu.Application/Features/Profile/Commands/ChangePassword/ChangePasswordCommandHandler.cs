namespace HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, string>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(
        IUserEFRepository userRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<string>(AuthErrors.InvalidCredentials);
        }

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<string>(AuthErrors.InvalidCredentials);
        }

        if (!_passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash))
        {
            return Result.Failure<string>(AuthErrors.InvalidOldPassword);
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(newPasswordHash);

        _userRepository.Update(user);

        return Result.Success("Thay đổi mật khẩu thành công.");
    }
}
