using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;

public sealed class TaoThePhuongTienCommandValidator : AbstractValidator<TaoThePhuongTienCommand>
{
    public TaoThePhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0).WithMessage("Giá trị Phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.MaThe)
            .NotEmpty().WithMessage("Mã thẻ không được để trống.")
            .MaximumLength(50).WithMessage("Mã thẻ không được vượt quá 50 ký tự.");
    }
}
