using FluentValidation;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.TaoThePhuongTien;

public sealed class TaoThePhuongTienCommandValidator : AbstractValidator<TaoThePhuongTienCommand>
{
    public TaoThePhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0)
            .WithMessage("Phương tiện không hợp lệ.");

        RuleFor(x => x.MaThe)
            .NotEmpty()
            .WithMessage("Mã thẻ không được để trống.")
            .MaximumLength(50)
            .WithMessage("Mã thẻ không được vượt quá 50 ký tự.");
    }
}
