using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;

public class XoaYeuCauCuTruCommandValidator : AbstractValidator<XoaYeuCauCuTruCommand>
{
    public XoaYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách yêu cầu cư trú không được để trống.");

        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage("Giá trị Yêu cầu cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
