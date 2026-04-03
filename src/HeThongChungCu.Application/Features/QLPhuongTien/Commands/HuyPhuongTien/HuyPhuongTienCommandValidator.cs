using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.HuyPhuongTien;

public sealed class HuyPhuongTienCommandValidator : AbstractValidator<HuyPhuongTienCommand>
{
    public HuyPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty().WithMessage(PhuongTienErrors.PhuongTienIdsNotEmpty.Description);

        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0).WithMessage(PhuongTienErrors.PhuongTienIdRange.Description);
    }
}
