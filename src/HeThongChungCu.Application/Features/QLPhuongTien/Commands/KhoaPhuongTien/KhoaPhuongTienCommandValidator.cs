using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

public sealed class KhoaPhuongTienCommandValidator : AbstractValidator<KhoaPhuongTienCommand>
{
    public KhoaPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty().WithMessage("Danh sách phương tiện không được để trống.");

        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0).WithMessage("Giá trị Phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
