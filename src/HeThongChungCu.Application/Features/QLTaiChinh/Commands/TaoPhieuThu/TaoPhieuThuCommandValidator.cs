using FluentValidation;
using HeThongChungCu.Domain.Enums;
using System;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuThu;

public class TaoPhieuThuCommandValidator : AbstractValidator<TaoPhieuThuCommand>
{
    public TaoPhieuThuCommandValidator()
    {
        RuleFor(x => x.NguoiGiaoDich)
            .NotEmpty().WithMessage("Người giao dịch không được để trống.")
            .MaximumLength(200).WithMessage("Người giao dịch không vượt quá 200 ký tự.");

        RuleFor(x => x.NgayGiaoDich)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("Ngày giao dịch không thể ở tương lai.");

        RuleFor(x => x.PhuongThucThanhToanId)
            .Must(id => PhuongThucThanhToan.ToDictionary().ContainsKey(id))
            .WithMessage("Phương thức thanh toán không hợp lệ.");

        RuleFor(x => x.ChiTiets)
            .NotEmpty().WithMessage("Phiếu thu phải có ít nhất một chi tiết.");

        RuleForEach(x => x.ChiTiets).ChildRules(details =>
        {
            details.RuleFor(x => x.SoTien)
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");
        });
    }
}
