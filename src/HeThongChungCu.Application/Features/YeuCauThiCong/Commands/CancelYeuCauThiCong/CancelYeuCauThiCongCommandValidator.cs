using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CancelYeuCauThiCong;

public class CancelYeuCauThiCongCommandValidator : AbstractValidator<CancelYeuCauThiCongCommand>
{
    public CancelYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.LyDo).NotEmpty().MaximumLength(1000).WithMessage("Lý do hủy tối đa 1000 ký tự.");
    }
}
