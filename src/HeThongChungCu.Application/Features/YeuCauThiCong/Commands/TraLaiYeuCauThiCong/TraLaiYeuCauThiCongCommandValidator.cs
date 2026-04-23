using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.TraLaiYeuCauThiCong;

public class TraLaiYeuCauThiCongCommandValidator : AbstractValidator<TraLaiYeuCauThiCongCommand>
{
    public TraLaiYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id không được để trống.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do không được để trống.")
            .MaximumLength(1000).WithMessage("Lý do phải có độ dài từ 1 đến 1000 ký tự.");
    }
}
