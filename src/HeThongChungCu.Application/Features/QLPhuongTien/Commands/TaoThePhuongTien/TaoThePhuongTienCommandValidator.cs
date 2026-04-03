using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;

public sealed class TaoThePhuongTienCommandValidator : AbstractValidator<TaoThePhuongTienCommand>
{
    public TaoThePhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0).WithMessage(PhuongTienErrors.PhuongTienIdRange.Description);

        RuleFor(x => x.MaThe)
            .NotEmpty().WithMessage(PhuongTienErrors.MaTheNotEmpty.Description)
            .MaximumLength(50).WithMessage(PhuongTienErrors.MaTheMaxLength.Description);
    }
}
