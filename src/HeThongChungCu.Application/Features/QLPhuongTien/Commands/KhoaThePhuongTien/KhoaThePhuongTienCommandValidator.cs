using FluentValidation;
using HeThongChungCu.Domain.Errors;

using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien;

public class KhoaThePhuongTienCommandValidator : AbstractValidator<KhoaThePhuongTienCommand>
{
    public KhoaThePhuongTienCommandValidator()
    {
        RuleFor(x => x.TheIds)
            .NotEmpty().WithMessage(PhuongTienErrors.TheIdsNotEmpty.Description);

        RuleForEach(x => x.TheIds)
            .GreaterThan(0).WithMessage(PhuongTienErrors.TheIdRange.Description);
    }
}
