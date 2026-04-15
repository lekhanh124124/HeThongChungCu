using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;

public class TuChoiYeuCauPhuongTienCommandValidator : AbstractValidator<TuChoiYeuCauPhuongTienCommand>
{
    public TuChoiYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.YeuCauPhuongTienId)
            .NotEmpty().WithMessage("Giá trị Yêu cầu phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do không được để trống.");
    }
}
