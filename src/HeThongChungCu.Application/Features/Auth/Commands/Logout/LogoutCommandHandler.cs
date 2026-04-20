using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.AccountId;
        if (accountId is null)
        {
            return AuthErrors.InvalidToken;
        }

        var account = await _accountRepository.GetWithTokensAsync(accountId.Value, cancellationToken);

        if (account is null)
        {
            return AuthErrors.InvalidToken;
        }

        var activeTokens = account.Tokens.Where(t => t.IsActive).ToList();

        foreach (var token in activeTokens)
        {
            account.RevokeToken(token.TokenHash, _dateTimeProvider.UtcNow, ReasonRevoked.Logout);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
