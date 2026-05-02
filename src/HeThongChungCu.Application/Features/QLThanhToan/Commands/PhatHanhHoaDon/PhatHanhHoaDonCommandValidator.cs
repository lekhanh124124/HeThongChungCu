using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.PhatHanhHoaDon;

public class PhatHanhHoaDonCommandValidator : AbstractValidator<PhatHanhHoaDonCommand>
{
    public PhatHanhHoaDonCommandValidator()
    {
        RuleFor(x => x.DotThanhToanId)
            .NotEmpty().WithMessage("Mã đợt thanh toán không được để trống.");
    }
}
