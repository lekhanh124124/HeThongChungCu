using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;

public class NhapBaoGiaYeuCauSuaChuaCommandValidator : AbstractValidator<NhapBaoGiaYeuCauSuaChuaCommand>
{
    public NhapBaoGiaYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.ChiPhiDuKien)
            .GreaterThanOrEqualTo(0).WithMessage("Chi phí dự kiến không được âm.");

        When(x => !x.IsMienPhi, () =>
        {
            RuleFor(x => x.ChiPhiDuKien)
                .GreaterThan(0).WithMessage("Chi phí dự kiến phải lớn hơn 0 nếu không miễn phí.");
        });

        RuleFor(x => x.GhiChuBaoGia)
            .MaximumLength(1000).WithMessage("Ghi chú báo giá không được vượt quá 1000 ký tự.");

        RuleFor(x => x)
            .Must(x => x.IsMienPhi || x.ChiPhiDuKien > 0)
            .WithMessage("Nếu không miễn phí, chi phí dự kiến phải lớn hơn 0.");
    }
}
