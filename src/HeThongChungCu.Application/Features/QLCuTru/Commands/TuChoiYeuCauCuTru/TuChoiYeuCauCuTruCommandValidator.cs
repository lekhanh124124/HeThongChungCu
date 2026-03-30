namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public class TuChoiYeuCauCuTruCommandValidator : AbstractValidator<TuChoiYeuCauCuTruCommand>
{
    public TuChoiYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.YeuCauCuTruId)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do từ chối là bắt buộc.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }
}
