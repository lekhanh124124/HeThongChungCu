using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;

public class TiepNhanYeuCauSuaChuaCommandValidator : AbstractValidator<TiepNhanYeuCauSuaChuaCommand>
{
    public TiepNhanYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.")
            .GreaterThan(0).WithMessage("ID yêu cầu không hợp lệ.");
    }
}
