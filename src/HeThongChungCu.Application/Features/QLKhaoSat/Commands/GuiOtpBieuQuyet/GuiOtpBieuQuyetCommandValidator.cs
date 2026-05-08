using FluentValidation;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.GuiOtpBieuQuyet;

public class GuiOtpBieuQuyetCommandValidator : AbstractValidator<GuiOtpBieuQuyetCommand>
{
    public GuiOtpBieuQuyetCommandValidator()
    {
        RuleFor(x => x.KhaoSatId)
            .NotEmpty().WithMessage("KhaoSatId không được để trống.");

        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("CanHoId không được để trống.");

        RuleFor(x => x.NguoiDungId)
            .NotEmpty().WithMessage("NguoiDungId không được để trống.");
    }
}
