using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HenLichYeuCauSuaChua;

public class HenLichYeuCauSuaChuaCommandValidator : AbstractValidator<HenLichYeuCauSuaChuaCommand>
{
    public HenLichYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.HenTu)
            .NotEmpty().WithMessage("Thời gian hẹn bắt đầu không được để trống.");

        RuleFor(x => x.HenDen)
            .NotEmpty().WithMessage("Thời gian hẹn kết thúc không được để trống.")
            .GreaterThan(x => x.HenTu).WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
    }
}
