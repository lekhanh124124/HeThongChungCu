using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.NghiemThuThiCong;

public class NghiemThuThiCongCommandValidator : AbstractValidator<NghiemThuThiCongCommand>
{
    public NghiemThuThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
    }
}
