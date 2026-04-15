using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public class XoaYeuCauPhuongTienCommandValidator : AbstractValidator<XoaYeuCauPhuongTienCommand>
{
    public XoaYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách yêu cầu phương tiện không được để trống.");

        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage("Giá trị Yêu cầu phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
