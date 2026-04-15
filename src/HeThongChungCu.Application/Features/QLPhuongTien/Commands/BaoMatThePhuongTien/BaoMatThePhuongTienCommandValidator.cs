using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public class BaoMatThePhuongTienCommandValidator : AbstractValidator<BaoMatThePhuongTienCommand>
{
    public BaoMatThePhuongTienCommandValidator()
    {
        RuleForEach(x => x.TheIds)
            .GreaterThan(0).WithMessage("Giá trị Thẻ phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}

