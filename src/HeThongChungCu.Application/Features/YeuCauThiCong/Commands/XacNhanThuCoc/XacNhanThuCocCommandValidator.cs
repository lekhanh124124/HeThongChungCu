using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.XacNhanThuCoc;

public class XacNhanThuCocCommandValidator : AbstractValidator<XacNhanThuCocCommand>
{
    public XacNhanThuCocCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.GhiChu).MaximumLength(1000).WithMessage("Ghi chú tối đa 1000 ký tự.");
    }
}
