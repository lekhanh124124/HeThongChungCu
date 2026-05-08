using FluentValidation;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.XacNhanBieuQuyet;

public class XacNhanBieuQuyetCommandValidator : AbstractValidator<XacNhanBieuQuyetCommand>
{
    public XacNhanBieuQuyetCommandValidator()
    {
        RuleFor(x => x.KhaoSatId)
            .NotEmpty().WithMessage("KhaoSatId không được để trống.");

        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("CanHoId không được để trống.");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("Mã xác thực OTP không được để trống.")
            .Length(6).WithMessage("Mã OTP phải có độ dài đúng 6 ký tự số.");

        RuleFor(x => x.TraLois)
            .NotEmpty().WithMessage("Danh sách lựa chọn biểu quyết không được để trống.");

        RuleForEach(x => x.TraLois).ChildRules(ans =>
        {
            ans.RuleFor(a => a.LuaChonId)
                .NotEmpty().WithMessage("LuaChonId không được để trống.");
        });
    }
}
