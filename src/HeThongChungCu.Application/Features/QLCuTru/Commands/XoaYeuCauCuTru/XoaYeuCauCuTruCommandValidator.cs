using FluentValidation;
using HeThongChungCu.Domain.Errors;

using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandValidator : AbstractValidator<XoaYeuCauCuTruCommand>
{
    public XoaYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.YeuCauCuTruIdsNotEmpty.Description);

        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage(YeuCauCuTruErrors.YeuCauCuTruIdRange.Description);
    }
}
