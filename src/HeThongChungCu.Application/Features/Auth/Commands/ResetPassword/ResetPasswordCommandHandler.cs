using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, string>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly IHasherService _hasherService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        IHasherService hasherService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _hasherService = hasherService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByTenDangNhapAsync(request.Username, cancellationToken);
        if (account is null)
        {
            return UserErrors.NotFound;
        }

        if (_hasherService.VerifyPassword(request.NewPassword, account.MatKhauHash))
        {
            return AuthErrors.PasswordNotChanged;
        }

        var resetCodeHash = _hasherService.HashToken(request.ResetCode);
        var token = account.Tokens.FirstOrDefault(t =>
            t.TokenType == TokenType.ResetPasswordCode &&
            t.TokenHash == resetCodeHash &&
            t.IsActive);

        if (token is null)
        {
            return AuthErrors.InvalidResetToken;
        }

        var hashedPassword = _hasherService.HashPassword(request.NewPassword);
        account.UpdatePassword(hashedPassword);

        account.RevokeToken(token.TokenHash, _dateTimeProvider.UtcNow, ReasonRevoked.ReplacedByNewToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "Đổi mật khẩu thành công.";
    }
}
