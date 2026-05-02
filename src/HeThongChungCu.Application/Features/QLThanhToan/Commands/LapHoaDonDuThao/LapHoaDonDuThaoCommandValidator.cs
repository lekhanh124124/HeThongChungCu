using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class LapHoaDonDuThaoCommandValidator : AbstractValidator<LapHoaDonDuThaoCommand>
{
    public LapHoaDonDuThaoCommandValidator()
    {
        RuleFor(x => x.DotThanhToanId)
            .NotEmpty().WithMessage("Mã đợt thanh toán là bắt buộc.");
    }
}
