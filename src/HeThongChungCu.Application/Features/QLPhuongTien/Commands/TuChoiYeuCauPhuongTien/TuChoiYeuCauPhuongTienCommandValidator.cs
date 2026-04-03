using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;

public class TuChoiYeuCauPhuongTienCommandValidator : AbstractValidator<TuChoiYeuCauPhuongTienCommand>
{
    public TuChoiYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.YeuCauPhuongTienId)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdRange.Description);

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.LyDoNotEmpty.Description);
    }
}
