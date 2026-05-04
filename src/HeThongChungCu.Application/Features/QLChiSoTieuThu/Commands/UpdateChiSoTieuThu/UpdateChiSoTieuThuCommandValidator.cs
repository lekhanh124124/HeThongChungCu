using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UpdateChiSoTieuThu;

public class UpdateChiSoTieuThuCommandValidator : AbstractValidator<UpdateChiSoTieuThuCommand>
{
    public UpdateChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID chỉ số tiêu thụ phải lớn hơn 0.");

        RuleFor(x => x.ChiSoCu)
            .GreaterThanOrEqualTo(0).WithMessage("Chỉ số cũ không được nhỏ hơn 0.");

        RuleFor(x => x.ChiSoMoi)
            .GreaterThanOrEqualTo(0).WithMessage("Chỉ số mới không được nhỏ hơn 0.");

        RuleFor(x => x)
            .Must(x => x.ChiSoMoi >= x.ChiSoCu)
            .WithMessage("Chỉ số mới không được nhỏ hơn chỉ số cũ.");

        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .InclusiveBetween(2000, 2100).WithMessage("Năm phải từ 2000 đến 2100.");

        RuleFor(x => x.NgayGhiNhan)
            .NotEmpty()
            .WithMessage("Ngày ghi nhận không được để trống.");
    }
}
