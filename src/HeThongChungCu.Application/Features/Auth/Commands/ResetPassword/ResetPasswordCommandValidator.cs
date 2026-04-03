using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(AuthErrors.UsernameNotEmpty.Description);

        RuleFor(x => x.ResetCode)
            .NotEmpty().WithMessage(AuthErrors.ResetCodeNotEmpty.Description);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(AuthErrors.NewPasswordNotEmpty.Description)
            .MinimumLength(6).WithMessage(AuthErrors.PasswordMinLength(6).Description)
            .Matches(@"[A-Z]").WithMessage(AuthErrors.PasswordRequiresUpper.Description)
            .Matches(@"[a-z]").WithMessage(AuthErrors.PasswordRequiresLower.Description)
            .Matches(@"[0-9]").WithMessage(AuthErrors.PasswordRequiresDigit.Description)
            .Matches(@"[^a-zA-Z0-9]").WithMessage(AuthErrors.PasswordRequiresNonAlphanumeric.Description);


        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(AuthErrors.PasswordConfirmationMismatch.Description);
    }
}
