using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public class TuChoiYeuCauCuTruCommandValidator : AbstractValidator<TuChoiYeuCauCuTruCommand>
{
    public TuChoiYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.YeuCauCuTruId)
            .NotEmpty().WithMessage("Giá trị Yêu cầu cư trú phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do không được để trống.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }
}
