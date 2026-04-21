using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TraLaiYeuCauSuaChua;

public class TraLaiYeuCauSuaChuaCommandValidator : AbstractValidator<TraLaiYeuCauSuaChuaCommand>
{
    public TraLaiYeuCauSuaChuaCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(v => v.LyDo)
            .NotEmpty().WithMessage("Lý do yêu cầu bổ sung không được để trống.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }
}
