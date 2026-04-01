using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;

public sealed class TaoThePhuongTienCommandValidator : AbstractValidator<TaoThePhuongTienCommand>
{
    public TaoThePhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0)
            .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.MaThe)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50)
            .WithMessage(ValidationErrors.MaxLength(50).Description);
    }
}
