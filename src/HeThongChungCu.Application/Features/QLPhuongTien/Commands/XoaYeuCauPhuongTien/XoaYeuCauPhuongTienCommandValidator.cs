using FluentValidation;
using HeThongChungCu.Domain.Errors;

using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public class XoaYeuCauPhuongTienCommandValidator : AbstractValidator<XoaYeuCauPhuongTienCommand>
{
    public XoaYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdsNotEmpty.Description);

        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdRange.Description);
    }
}
