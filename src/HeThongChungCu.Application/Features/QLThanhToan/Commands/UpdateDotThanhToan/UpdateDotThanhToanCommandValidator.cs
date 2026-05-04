using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.UpdateDotThanhToan;

public class UpdateDotThanhToanCommandValidator : AbstractValidator<UpdateDotThanhToanCommand>
{
    public UpdateDotThanhToanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.TenDot)
            .NotEmpty().WithMessage("Tên đợt thanh toán không được để trống.")
            .MaximumLength(100).WithMessage("Tên đợt thanh toán không được quá 100 ký tự.");

        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .GreaterThan(2000).WithMessage("Năm không hợp lệ.");
            
        RuleFor(x => x.GhiChu)
            .MaximumLength(500).WithMessage("Ghi chú không được quá 500 ký tự.");
    }
}
