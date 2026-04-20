using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.PheDuyetYeuCauSuaChua;

public class PheDuyetYeuCauSuaChuaCommandValidator : AbstractValidator<PheDuyetYeuCauSuaChuaCommand>
{
    public PheDuyetYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.")
            .GreaterThan(0).WithMessage("ID yêu cầu không hợp lệ.");
    }
}
