using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHopDong;

public class CreateHopDongCommandValidator : AbstractValidator<CreateHopDongCommand>
{
    public CreateHopDongCommandValidator()
    {
        RuleFor(x => x.DoiTacId)
            .NotEmpty().WithErrorCode(DoiTacErrors.IdNotEmpty.Code).WithMessage(DoiTacErrors.IdNotEmpty.Description);

        RuleFor(x => x.HopDong).ChildRules(hopDong =>
        {
            hopDong.RuleFor(x => x.SoHopDong).NotEmpty().WithErrorCode(DoiTacErrors.SoHopDongNotEmpty.Code).WithMessage(DoiTacErrors.SoHopDongNotEmpty.Description);
            hopDong.RuleFor(x => x.NgayHetHan)
                .GreaterThan(x => x.NgayKy).WithErrorCode(DoiTacErrors.NgayHetHanInvalid.Code).WithMessage(DoiTacErrors.NgayHetHanInvalid.Description);
            hopDong.RuleFor(x => x.GiaTri).GreaterThanOrEqualTo(0).WithErrorCode(DoiTacErrors.GiaTriHopDongNegative.Code).WithMessage(DoiTacErrors.GiaTriHopDongNegative.Description);

            // Validate Service Fields directly on HopDongRequestDto
            hopDong.RuleFor(x => x.MaDichVu).NotEmpty().WithErrorCode(DoiTacErrors.MaDichVuNotEmpty.Code).WithMessage(DoiTacErrors.MaDichVuNotEmpty.Description);
            hopDong.RuleFor(x => x.TenDichVu).NotEmpty().WithErrorCode(DoiTacErrors.TenDichVuNotEmpty.Code).WithMessage(DoiTacErrors.TenDichVuNotEmpty.Description);
            hopDong.RuleFor(x => x.LoaiDichVuId).GreaterThan(0).WithErrorCode(DoiTacErrors.LoaiDichVuInvalid.Code).WithMessage(DoiTacErrors.LoaiDichVuInvalid.Description);
            hopDong.RuleFor(x => x.DonViTinh).NotEmpty().WithErrorCode(DoiTacErrors.DonViTinhNotEmpty.Code).WithMessage(DoiTacErrors.DonViTinhNotEmpty.Description);
            hopDong.RuleFor(x => x.SoLuongToiDa).GreaterThan(0).When(x => x.SoLuongToiDa != null).WithErrorCode(DoiTacErrors.SoLuongToiDaInvalid.Code).WithMessage(DoiTacErrors.SoLuongToiDaInvalid.Description);
        });
    }
}
