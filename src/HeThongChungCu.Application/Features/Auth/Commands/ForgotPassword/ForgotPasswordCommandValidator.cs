namespace HeThongChungCu.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên dang nh?p không du?c d? tr?ng.");
    }
}
