using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveTepThiCong;

public class RemoveTepThiCongCommandValidator : AbstractValidator<RemoveTepThiCongCommand>
{
    public RemoveTepThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.TepId).NotEmpty().WithMessage("TepId không được để trống.");
    }
}
