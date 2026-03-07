using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

namespace HeThongChungCu.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly IUserEFRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogoutCommandHandler(
        IUserEFRepository userRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            return Result.Failure<bool>(AuthErrors.InvalidToken);
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<bool>(AuthErrors.InvalidToken);
        }

        var activeTokens = user.Tokens.Where(t => t.IsActive).ToList();

        foreach (var token in activeTokens)
        {
            token.Revoke(_dateTimeProvider.UtcNow, ReasonRevoked.Logout);
        }

        return Result.Success(true);
    }
}
