using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DuyetDotThanhToan;

public class DuyetDotThanhToanCommandValidator : AbstractValidator<DuyetDotThanhToanCommand>
{
    public DuyetDotThanhToanCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.");
    }
}
