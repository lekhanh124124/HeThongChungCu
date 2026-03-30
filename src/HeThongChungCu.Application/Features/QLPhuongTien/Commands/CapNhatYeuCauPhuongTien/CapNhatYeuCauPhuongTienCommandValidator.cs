using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandValidator : AbstractValidator<CapNhatYeuCauPhuongTienCommand>
{
    public CapNhatYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.TenPhuongTien)
                .MaximumLength(100).WithMessage("Tên phương tiện không được vượt quá 100 ký tự.");

            RuleFor(x => x.BienSo)
                .MaximumLength(20).WithMessage("Biển số không được vượt quá 20 ký tự.");

            RuleFor(x => x.MauXe)
                .MaximumLength(50).WithMessage("Màu xe không được vượt quá 50 ký tự.");

            When(x => x.FileIds != null, () =>
            {
                RuleForEach(x => x.FileIds)
                    .GreaterThan(0).WithMessage("ID tệp tin không hợp lệ.");
            });
        });
    }
}
