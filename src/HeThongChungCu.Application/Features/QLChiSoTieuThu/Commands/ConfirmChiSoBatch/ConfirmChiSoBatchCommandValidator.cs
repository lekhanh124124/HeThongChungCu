using FluentValidation;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;

public class ConfirmChiSoBatchCommandValidator : AbstractValidator<ConfirmChiSoBatchCommand>
{
    public ConfirmChiSoBatchCommandValidator()
    {
        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .GreaterThanOrEqualTo(2000).WithMessage("Năm không hợp lệ.");
    }
}
