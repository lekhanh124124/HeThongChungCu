namespace HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên dang nh?p không du?c d? tr?ng.");

        RuleFor(x => x.ResetCode)
            .NotEmpty().WithMessage("Mã khôi ph?c không du?c d? tr?ng.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("M?t kh?u không du?c d? tr?ng.")
            .MinimumLength(6).WithMessage("M?t kh?u ph?i có ít nh?t 6 ký t?.")
            .Matches(@"[A-Z]").WithMessage("M?t kh?u ph?i ch?a ít nh?t m?t ch? cái vi?t hoa.")
            .Matches(@"[a-z]").WithMessage("M?t kh?u ph?i ch?a ít nh?t m?t ch? cái vi?t thu?ng.")
            .Matches(@"[0-9]").WithMessage("M?t kh?u ph?i ch?a ít nh?t m?t ch? s?.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("M?t kh?u ph?i ch?a ít nh?t m?t ký t? d?c bi?t.");


        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("M?t kh?u xác nh?n không kh?p.");
    }
}
