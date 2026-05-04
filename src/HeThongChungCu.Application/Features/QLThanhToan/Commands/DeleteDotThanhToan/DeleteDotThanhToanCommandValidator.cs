using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DeleteDotThanhToan;

public class DeleteDotThanhToanCommandValidator : AbstractValidator<DeleteDotThanhToanCommand>
{
    public DeleteDotThanhToanCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID cần xóa không được để trống.")
            .Must(x => x != null && x.Count > 0).WithMessage("Vui lòng chọn ít nhất một đợt thanh toán để xóa.");
    }
}
