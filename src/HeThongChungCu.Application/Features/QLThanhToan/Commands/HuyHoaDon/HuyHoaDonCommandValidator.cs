using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.HuyHoaDon;

public class HuyHoaDonCommandValidator : AbstractValidator<HuyHoaDonCommand>
{
    public HuyHoaDonCommandValidator()
    {
        RuleFor(x => x.HoaDonId)
            .NotEmpty().WithMessage("ID hóa đơn không được để trống.");

        RuleFor(x => x.LyDo)
            .MaximumLength(200).WithMessage("Lý do hủy tối đa 200 ký tự.");
    }
}
