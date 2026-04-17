using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecNoiBo;

public class GiaoViecNoiBoCommandValidator : AbstractValidator<GiaoViecNoiBoCommand>
{
    public GiaoViecNoiBoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.NhanVienId)
            .NotEmpty().WithMessage("ID nhân viên không được để trống.");
    }
}
