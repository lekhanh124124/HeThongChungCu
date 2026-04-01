using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

public sealed class KhoaPhuongTienCommandValidator : AbstractValidator<KhoaPhuongTienCommand>
{
    public KhoaPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
            
        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
    }
}
