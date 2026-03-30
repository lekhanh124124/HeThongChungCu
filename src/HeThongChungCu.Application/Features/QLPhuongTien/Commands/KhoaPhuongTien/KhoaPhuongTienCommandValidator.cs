using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;

public sealed class KhoaPhuongTienCommandValidator : AbstractValidator<KhoaPhuongTienCommand>
{
    public KhoaPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty().WithMessage("Danh sách ID phương tiện không được để trống.");
            
        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0).WithMessage("ID phương tiện không hợp lệ.");
    }
}
