using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using System.Security.Cryptography;

namespace HeThongChungCu.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, string>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserEFRepository userRepository,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        var resetCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var expiresAt = _dateTimeProvider.UtcNow.AddMinutes(15);

        var resetToken = Tokens.CreateResetPasswordToken(user.Id, resetCode, expiresAt);
        user.AddToken(resetToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetCode, cancellationToken);

        var maskedEmail = _emailService.MaskEmail(user.Email);

        return Result.Success($"Mã khôi phục đã được gửi thành công đến email {maskedEmail}");
    }
}
