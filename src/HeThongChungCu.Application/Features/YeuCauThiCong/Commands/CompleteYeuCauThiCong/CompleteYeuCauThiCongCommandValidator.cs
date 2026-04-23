using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CompleteYeuCauThiCong;

public class CompleteYeuCauThiCongCommandValidator : AbstractValidator<CompleteYeuCauThiCongCommand>
{
    public CompleteYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
    }
}
