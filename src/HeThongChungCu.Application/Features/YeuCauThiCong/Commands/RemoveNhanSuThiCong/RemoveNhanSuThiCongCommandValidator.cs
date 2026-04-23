using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveNhanSuThiCong;

public class RemoveNhanSuThiCongCommandValidator : AbstractValidator<RemoveNhanSuThiCongCommand>
{
    public RemoveNhanSuThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.NhanSuId).NotEmpty().WithMessage("NhanSuId không được để trống.");
        RuleFor(x => x.LyDo).NotEmpty().MaximumLength(500).WithMessage("Lý do không được để trống và tối đa 500 ký tự.");
    }
}
