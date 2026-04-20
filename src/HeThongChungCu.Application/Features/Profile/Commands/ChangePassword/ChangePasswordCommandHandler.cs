using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, string>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly IHasherService _hasherService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        IHasherService hasherService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _hasherService = hasherService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.AccountId is null)
        {
            return AuthErrors.InvalidCredentials;
        }

        var account = await _accountRepository.GetByIdAsync(_currentUserService.AccountId.Value, cancellationToken);
        if (account is null)
        {
            return AuthErrors.InvalidCredentials;
        }

        if (!_hasherService.VerifyPassword(request.OldPassword, account.MatKhauHash))
        {
            return AuthErrors.InvalidOldPassword;
        }

        var newPasswordHash = _hasherService.HashPassword(request.NewPassword);
        account.UpdatePassword(newPasswordHash);

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "Thay đổi mật khẩu thành công.";
    }
}
