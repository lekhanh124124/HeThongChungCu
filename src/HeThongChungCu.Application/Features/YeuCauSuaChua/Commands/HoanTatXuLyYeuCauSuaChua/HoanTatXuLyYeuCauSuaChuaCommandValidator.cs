using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HoanTatXuLyYeuCauSuaChua;

public class HoanTatXuLyYeuCauSuaChuaCommandValidator : AbstractValidator<HoanTatXuLyYeuCauSuaChuaCommand>
{
    public HoanTatXuLyYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.KetQuaXuLy)
            .NotEmpty().WithMessage("Kết quả xử lý không được để trống.")
            .MaximumLength(2000).WithMessage("Kết quả xử lý không được vượt quá 2000 ký tự.");

        RuleFor(x => x.ChiPhiThucTe)
            .GreaterThanOrEqualTo(0).When(x => x.ChiPhiThucTe.HasValue)
            .WithMessage("Chi phí thực tế không được âm.");
    }
}
