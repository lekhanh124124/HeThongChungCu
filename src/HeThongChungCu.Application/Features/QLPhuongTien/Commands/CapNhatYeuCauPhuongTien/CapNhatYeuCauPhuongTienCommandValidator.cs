using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandValidator : AbstractValidator<CapNhatYeuCauPhuongTienCommand>
{
    public CapNhatYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdRange.Description);

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.YeuCauTenPhuongTien)
                .MaximumLength(100).WithMessage(PhuongTienErrors.TenXeMaxLength.Description);

            RuleFor(x => x.YeuCauBienSo)
                .MaximumLength(20).WithMessage(PhuongTienErrors.BienSoMaxLength.Description);

            RuleFor(x => x.YeuCauMauXe)
                .MaximumLength(50).WithMessage(PhuongTienErrors.MauXeMaxLength.Description);

            When(x => x.FileIds != null, () =>
            {
                RuleForEach(x => x.FileIds)
                    .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.FileIdRange.Description);
            });
        });
    }
}
