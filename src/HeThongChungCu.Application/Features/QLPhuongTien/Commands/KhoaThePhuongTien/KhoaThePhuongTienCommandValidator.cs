using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien;

public class KhoaThePhuongTienCommandValidator : AbstractValidator<KhoaThePhuongTienCommand>
{
    public KhoaThePhuongTienCommandValidator()
    {
        RuleFor(x => x.TheIds)
            .NotNull().WithMessage(ValidationErrors.NotEmpty.Description)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}

