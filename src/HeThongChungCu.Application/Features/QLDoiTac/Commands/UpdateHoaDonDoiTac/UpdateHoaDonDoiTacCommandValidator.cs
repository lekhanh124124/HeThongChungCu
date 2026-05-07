using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateHoaDonDoiTac;

public class UpdateHoaDonDoiTacCommandValidator : AbstractValidator<UpdateHoaDonDoiTacCommand>
{
    public UpdateHoaDonDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithErrorCode("Validation.InvalidId").WithMessage("ID hóa đơn đối tác không hợp lệ.");

        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithErrorCode("Validation.InvalidMonth").WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .GreaterThan(2000).WithErrorCode("Validation.InvalidYear").WithMessage("Năm không hợp lệ.");

        RuleFor(x => x.SoTien)
            .GreaterThanOrEqualTo(0).WithErrorCode("Validation.NegativeAmount").WithMessage("Số tiền không được phép âm.");

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000).WithErrorCode("Validation.MaxLength").WithMessage("Ghi chú không được vượt quá 1000 ký tự.");
    }
}
