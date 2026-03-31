using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;

public class TuChoiYeuCauPhuongTienCommandValidator : AbstractValidator<TuChoiYeuCauPhuongTienCommand>
{
    public TuChoiYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.YeuCauPhuongTienId)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do từ chối không được để trống.");
    }
}
