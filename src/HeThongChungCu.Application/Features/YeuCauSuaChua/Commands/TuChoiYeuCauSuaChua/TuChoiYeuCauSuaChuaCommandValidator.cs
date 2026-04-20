using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TuChoiYeuCauSuaChua;

public class TuChoiYeuCauSuaChuaCommandValidator : AbstractValidator<TuChoiYeuCauSuaChuaCommand>
{
    public TuChoiYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.")
            .GreaterThan(0).WithMessage("ID yêu cầu không hợp lệ.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Cần cung cấp lý do từ chối.")
            .MaximumLength(500).WithMessage("Lý do từ chối không được vượt quá 500 ký tự.");
    }
}
