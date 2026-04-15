using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;

public class CapNhatYeuCauCuTruCommandValidator : AbstractValidator<CapNhatYeuCauCuTruCommand>
{
    public CapNhatYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Giá trị Yêu cầu cư trú phải nằm trong khoảng từ 1 đến 2147483647.")
            .GreaterThan(0).WithMessage("Giá trị Yêu cầu cư trú phải nằm trong khoảng từ 1 đến 2147483647.");

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự.");
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự.");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");
            RuleFor(x => x.CCCD)
                .MaximumLength(50).WithMessage("CCCD/CMND không được vượt quá 50 ký tự.");
            RuleFor(x => x.DiaChi)
                .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự.");
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty().WithMessage("Giá trị Giấy tờ phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Giấy tờ phải nằm trong khoảng từ 1 đến 2147483647.");
            attachment.RuleFor(a => a.SoGiayTo)
                .NotEmpty().WithMessage("Số giấy tờ không được để trống.")
                .MaximumLength(100).WithMessage("Số giấy tờ không được vượt quá 100 ký tự.");
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty().WithMessage("Tệp tin đính kèm không được để trống.");
        });
    }
}
