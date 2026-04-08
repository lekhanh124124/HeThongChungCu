using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage(AuthErrors.CurrentPasswordNotEmpty.Description);

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

