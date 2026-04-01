using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public class BaoMatThePhuongTienCommandValidator : AbstractValidator<BaoMatThePhuongTienCommand>
{
    public BaoMatThePhuongTienCommandValidator()
    {
        RuleFor(x => x.TheIds)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}

