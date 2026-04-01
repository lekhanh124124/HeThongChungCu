using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public class XoaYeuCauPhuongTienCommandValidator : AbstractValidator<XoaYeuCauPhuongTienCommand>
{
    public XoaYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
    }
}
