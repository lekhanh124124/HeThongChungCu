using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public class BaoMatThePhuongTienCommandValidator : AbstractValidator<BaoMatThePhuongTienCommand>
{
    public BaoMatThePhuongTienCommandValidator()
    {
        RuleForEach(x => x.TheIds)
            .GreaterThan(0).WithMessage(PhuongTienErrors.TheIdRange.Description);
    }
}

