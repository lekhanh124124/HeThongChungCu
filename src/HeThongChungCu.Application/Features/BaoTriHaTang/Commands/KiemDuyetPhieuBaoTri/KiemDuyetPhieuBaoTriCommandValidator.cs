using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.KiemDuyetPhieuBaoTri;

public class KiemDuyetPhieuBaoTriCommandValidator : AbstractValidator<KiemDuyetPhieuBaoTriCommand>
{
    public KiemDuyetPhieuBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID phiếu bảo trì không được để trống.");


        RuleFor(x => x.GhiChuXuLy)
            .NotEmpty().When(x => !x.IsDuyet)
            .WithMessage("Phải cung cấp lý do/phản hồi khi từ chối nghiệm thu.")
            .MaximumLength(1000).WithMessage("Phản hồi không vượt quá 1000 ký tự.");
    }
}
