using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public class TuChoiYeuCauCuTruCommandValidator : AbstractValidator<TuChoiYeuCauCuTruCommand>
{
    public TuChoiYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.YeuCauCuTruId)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.LyDoNotEmpty.Description)
            .MaximumLength(500).WithMessage(ValidationErrors.MaxLength(500).Description);
    }
}
