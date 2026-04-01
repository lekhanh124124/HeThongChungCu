using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandValidator : AbstractValidator<XoaYeuCauCuTruCommand>
{
    public XoaYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}
