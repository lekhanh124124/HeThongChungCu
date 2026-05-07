using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.GhiNhanGiaoDichThanhToan;

public class GhiNhanGiaoDichThanhToanCommandValidator : AbstractValidator<GhiNhanGiaoDichThanhToanCommand>
{
    public GhiNhanGiaoDichThanhToanCommandValidator()
    {
        RuleFor(x => x.HoaDonId)
            .GreaterThan(0);

        RuleFor(x => x.ChiTietHoaDonIds)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage("ChiTietHoaDonIds không được rỗng.")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("ChiTietHoaDonIds không được trùng lặp.");

        RuleFor(x => x.PhuongThucThanhToanId)
            .Must(v => PhuongThucThanhToan.FromValue(v, null) != null)
            .WithMessage("PhuongThucThanhToanId không hợp lệ.");

        RuleFor(x => x.MaGiaoDich)
            .MaximumLength(100);

        RuleFor(x => x.GhiChu)
            .MaximumLength(500);
    }
}
