using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;

public class HuyYeuCauSuaChuaCommandValidator : AbstractValidator<HuyYeuCauSuaChuaCommand>
{
    public HuyYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.LyDoHuy)
            .NotEmpty().WithMessage("Lý do hủy không được để trống.")
            .MaximumLength(1000).WithMessage("Lý do hủy không được vượt quá 1000 ký tự.");
    }
}
