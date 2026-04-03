using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KichHoatPhuongTien;

public sealed class KichHoatPhuongTienCommandValidator : AbstractValidator<KichHoatPhuongTienCommand>
{
    public KichHoatPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty().WithMessage(PhuongTienErrors.PhuongTienIdsNotEmpty.Description);

        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0).WithMessage(PhuongTienErrors.PhuongTienIdRange.Description);
    }
}
