using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.HuyPhieuBaoTri;

public class HuyPhieuBaoTriCommandValidator : AbstractValidator<HuyPhieuBaoTriCommand>
{
    public HuyPhieuBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID phiếu bảo trì không được để trống.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Lý do hủy phiếu không được để trống.")
            .MaximumLength(500).WithMessage("Lý do hủy phiếu không vượt quá 500 ký tự.");
    }
}
