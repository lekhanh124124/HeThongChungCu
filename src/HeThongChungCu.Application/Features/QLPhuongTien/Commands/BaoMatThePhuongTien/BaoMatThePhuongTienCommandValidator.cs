using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public class BaoMatThePhuongTienCommandValidator : AbstractValidator<BaoMatThePhuongTienCommand>
{
    public BaoMatThePhuongTienCommandValidator()
    {
        RuleFor(x => x.TheIds)
            .NotEmpty().WithMessage("Danh sách ID thẻ không được để trống.");
    }
}
