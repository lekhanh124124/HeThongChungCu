using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DongDotThanhToan;

public class DongDotThanhToanCommandValidator : AbstractValidator<DongDotThanhToanCommand>
{
    public DongDotThanhToanCommandValidator()
    {
        RuleFor(x => x.DotThanhToanId)
            .NotEmpty().WithMessage("ID đợt thanh toán không được để trống.");
    }
}
