using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public class XoaYeuCauPhuongTienCommandValidator : AbstractValidator<XoaYeuCauPhuongTienCommand>
{
    public XoaYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");
    }
}
