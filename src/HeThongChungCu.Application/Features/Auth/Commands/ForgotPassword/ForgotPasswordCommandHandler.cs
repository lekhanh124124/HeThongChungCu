using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using System.Security.Cryptography;

namespace HeThongChungCu.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, string>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly IEmailService _emailService;
    private readonly IHasherService _hasherService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        IEmailService emailService,
        IHasherService hasherService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _emailService = emailService;
        _hasherService = hasherService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByTenDangNhapAsync(request.Username, cancellationToken);

        if (account is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        var resetCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var expiresAt = _dateTimeProvider.UtcNow.AddMinutes(15);
        var resetCodeHash = _hasherService.HashToken(resetCode);
 
        account.AddResetPasswordToken(resetCodeHash, expiresAt);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(account.Email, resetCode, cancellationToken);

        var maskedEmail = _emailService.MaskEmail(account.Email);

        return Result.Success($"Mã khôi phục đã được gửi thành công đến email {maskedEmail}");
    }
}
