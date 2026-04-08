using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateDoiTac;

public class CreateDoiTacCommandValidator : AbstractValidator<CreateDoiTacCommand>
{
    public CreateDoiTacCommandValidator()
    {
        RuleFor(x => x.TenDoiTac)
            .NotEmpty().WithErrorCode(DoiTacErrors.TenDoiTacNotEmpty.Code).WithMessage(DoiTacErrors.TenDoiTacNotEmpty.Description)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.TenDoiTacMaxLength.Code).WithMessage(DoiTacErrors.TenDoiTacMaxLength.Description);

        RuleFor(x => x.TenCongTy)
            .MaximumLength(200).WithErrorCode(DoiTacErrors.TenCongTyMaxLength.Code).WithMessage(DoiTacErrors.TenCongTyMaxLength.Description);

        RuleFor(x => x.NguoiDaiDien)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.NguoiDaiDienMaxLength.Code).WithMessage(DoiTacErrors.NguoiDaiDienMaxLength.Description);

        RuleFor(x => x.SoGiayPhepKD)
            .MaximumLength(50).WithErrorCode(DoiTacErrors.SoGiayPhepKDMaxLength.Code).WithMessage(DoiTacErrors.SoGiayPhepKDMaxLength.Description);

        RuleFor(x => x.MaSoThue)
            .MaximumLength(50).WithErrorCode(DoiTacErrors.MaSoThueMaxLength.Code).WithMessage(DoiTacErrors.MaSoThueMaxLength.Description);
        
        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithErrorCode(DoiTacErrors.SoDienThoaiMaxLength.Code).WithMessage(DoiTacErrors.SoDienThoaiMaxLength.Description);

        RuleFor(x => x.Email)
            .MaximumLength(100).WithErrorCode(DoiTacErrors.EmailMaxLength.Code).WithMessage(DoiTacErrors.EmailMaxLength.Description);

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000).WithErrorCode(DoiTacErrors.GhiChuMaxLength.Code).WithMessage(DoiTacErrors.GhiChuMaxLength.Description);

        RuleFor(x => x.HopDongs)
            .NotEmpty().WithErrorCode(DoiTacErrors.HopDongsNotEmpty.Code).WithMessage(DoiTacErrors.HopDongsNotEmpty.Description);

        RuleForEach(x => x.HopDongs).ChildRules(hopDong =>
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
