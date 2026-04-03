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
            .NotEqual(x => x.OldPassword).WithMessage(AuthErrors.PasswordNotChanged.Description);


        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(AuthErrors.PasswordConfirmationMismatch.Description);
    }
}

