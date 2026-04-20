using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.XoaNhanSuSuaChua;

public class XoaNhanSuSuaChuaCommandValidator : AbstractValidator<XoaNhanSuSuaChuaCommand>
{
    public XoaNhanSuSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NhanSuId).NotEmpty();
        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Cần cung cấp lý do xóa nhân sự.")
            .MaximumLength(500).WithMessage("Lý do xóa không được vượt quá 500 ký tự.");
    }
}
