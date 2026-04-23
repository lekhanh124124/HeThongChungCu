using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.ApproveYeuCauThiCong;

public class ApproveYeuCauThiCongCommandValidator : AbstractValidator<ApproveYeuCauThiCongCommand>
{
    public ApproveYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
    }
}
