using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.TiepNhanVaPhanCong;

public class TiepNhanVaPhanCongCommandValidator : AbstractValidator<TiepNhanVaPhanCongCommand>
{
    public TiepNhanVaPhanCongCommandValidator()
    {
        RuleFor(x => x.PhanAnhId)
            .NotEmpty().WithMessage("PhanAnhId không được để trống.");
    }
}
