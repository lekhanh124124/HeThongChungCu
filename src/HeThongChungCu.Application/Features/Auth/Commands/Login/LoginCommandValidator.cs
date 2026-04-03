using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(AuthErrors.UsernameNotEmpty.Description);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(AuthErrors.PasswordNotEmpty.Description);
    }
}

