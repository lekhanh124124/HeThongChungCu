namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandValidator : AbstractValidator<XoaYeuCauCuTruCommand>
{
    public XoaYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");
    }
}