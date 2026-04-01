using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandValidator : AbstractValidator<CapNhatYeuCauPhuongTienCommand>
{
    public CapNhatYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.YeuCauTenPhuongTien)
                .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);

            RuleFor(x => x.YeuCauBienSo)
                .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);

            RuleFor(x => x.YeuCauMauXe)
                .MaximumLength(50).WithMessage(ValidationErrors.MaxLength(50).Description);

            When(x => x.FileIds != null, () =>
            {
                RuleForEach(x => x.FileIds)
                    .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
            });
        });
    }
}
