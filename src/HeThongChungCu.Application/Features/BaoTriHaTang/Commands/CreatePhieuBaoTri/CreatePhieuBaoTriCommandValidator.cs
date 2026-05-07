using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreatePhieuBaoTri;

public class CreatePhieuBaoTriCommandValidator : AbstractValidator<CreatePhieuBaoTriCommand>
{
    public CreatePhieuBaoTriCommandValidator()
    {
        RuleFor(x => x.MaPhieu)
            .NotEmpty().WithMessage("Mã phiếu không được để trống.")
            .MaximumLength(50).WithMessage("Mã phiếu không vượt quá 50 ký tự.");

        RuleFor(x => x.ThietBiId)
            .GreaterThan(0).WithMessage("ID thiết bị không hợp lệ.");

        RuleFor(x => x.HangMucBaoTriId)
            .GreaterThan(0).WithMessage("ID hạng mục bảo trì không hợp lệ.");

        RuleFor(x => x.NgayDuKien)
            .NotEmpty().WithMessage("Ngày dự kiến không được để trống.");
    }
}
