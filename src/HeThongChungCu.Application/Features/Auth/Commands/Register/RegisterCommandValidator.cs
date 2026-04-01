using FluentValidation;
using HeThongChungCu.Application.Common.Interfaces;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public RegisterCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .EmailAddress().WithMessage(ValidationErrors.InvalidEmail.Description);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MinimumLength(6).WithMessage(ValidationErrors.MinLength(6).Description)
            .Matches(@"[A-Z]").WithMessage(AuthErrors.PasswordRequiresUpper.Description)
            .Matches(@"[a-z]").WithMessage(AuthErrors.PasswordRequiresLower.Description)
            .Matches(@"[0-9]").WithMessage(AuthErrors.PasswordRequiresDigit.Description)
            .Matches(@"[^a-zA-Z0-9]").WithMessage(AuthErrors.PasswordRequiresNonAlphanumeric.Description);


        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .Equal(x => x.Password).WithMessage(AuthErrors.PasswordConfirmationMismatch.Description);
    }
}
