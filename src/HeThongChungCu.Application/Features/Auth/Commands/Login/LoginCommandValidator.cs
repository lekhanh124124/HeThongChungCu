namespace HeThongChungCu.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên dang nh?p không du?c d? tr?ng.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("M?t kh?u không du?c d? tr?ng.");
    }
}
