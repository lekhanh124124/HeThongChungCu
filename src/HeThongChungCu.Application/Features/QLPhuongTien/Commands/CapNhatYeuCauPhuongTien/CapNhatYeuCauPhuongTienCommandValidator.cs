using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public class CapNhatYeuCauPhuongTienCommandValidator : AbstractValidator<CapNhatYeuCauPhuongTienCommand>
{
    public CapNhatYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Giá trị Yêu cầu phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.YeuCauTenPhuongTien)
                .MaximumLength(100).WithMessage("Tên xe không được vượt quá 100 ký tự.");

            RuleFor(x => x.YeuCauBienSo)
                .MaximumLength(20).WithMessage("Biển số không được vượt quá 20 ký tự.");

            RuleFor(x => x.YeuCauMauXe)
                .MaximumLength(50).WithMessage("Màu xe không được vượt quá 50 ký tự.");

            When(x => x.FileIds != null, () =>
            {
                RuleForEach(x => x.FileIds)
                    .GreaterThan(0).WithMessage("Giá trị Tệp tin phải nằm trong khoảng từ 1 đến 2147483647.");
            });
        });
    }
}
