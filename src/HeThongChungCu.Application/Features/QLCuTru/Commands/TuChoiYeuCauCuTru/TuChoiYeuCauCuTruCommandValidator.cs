using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public class TuChoiYeuCauCuTruCommandValidator : AbstractValidator<TuChoiYeuCauCuTruCommand>
{
    public TuChoiYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.YeuCauCuTruId)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.YeuCauCuTruIdRange.Description);

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.LyDoNotEmpty.Description)
            .MaximumLength(500).WithMessage(YeuCauCuTruErrors.LyDoMaxLength.Description);
    }
}
