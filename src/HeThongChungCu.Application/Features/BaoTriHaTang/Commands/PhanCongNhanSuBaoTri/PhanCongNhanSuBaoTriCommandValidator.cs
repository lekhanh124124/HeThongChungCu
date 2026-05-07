using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.PhanCongNhanSuBaoTri;

public class PhanCongNhanSuBaoTriCommandValidator : AbstractValidator<PhanCongNhanSuBaoTriCommand>
{
    public PhanCongNhanSuBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID phiếu bảo trì không hợp lệ.");

        RuleFor(x => x.NgayDuKien)
            .NotEmpty().WithMessage("Ngày dự kiến không được để trống.");
    }
}
