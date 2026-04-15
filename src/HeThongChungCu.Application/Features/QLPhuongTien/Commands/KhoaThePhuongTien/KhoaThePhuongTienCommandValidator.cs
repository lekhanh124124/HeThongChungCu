using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien;

public class KhoaThePhuongTienCommandValidator : AbstractValidator<KhoaThePhuongTienCommand>
{
    public KhoaThePhuongTienCommandValidator()
    {
        RuleFor(x => x.TheIds)
            .NotEmpty().WithMessage("Danh sách thẻ phương tiện không được để trống.");

        RuleForEach(x => x.TheIds)
            .GreaterThan(0).WithMessage("Giá trị Thẻ phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
