using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateDoiTac;

public class CreateDoiTacCommandValidator : AbstractValidator<CreateDoiTacCommand>
{
    public CreateDoiTacCommandValidator()
    {
        RuleFor(x => x.TenDoiTac)
            .NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Tên đơn vị cung cấp không được để trống.")
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Tên đơn vị cung cấp không được vượt quá 100 ký tự.");

        RuleFor(x => x.TenCongTy)
            .MaximumLength(200).WithErrorCode("Validation.MaxLength").WithMessage("Tên công ty không được vượt quá 200 ký tự.");

        RuleFor(x => x.NguoiDaiDien)
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Người đại diện không được vượt quá 100 ký tự.");

        RuleFor(x => x.SoGiayPhepKD)
            .MaximumLength(50).WithErrorCode("Validation.MaxLength").WithMessage("Số giấy phép kinh doanh không được vượt quá 50 ký tự.");

        RuleFor(x => x.MaSoThue)
            .MaximumLength(50).WithErrorCode("Validation.MaxLength").WithMessage("Mã số thuế không được vượt quá 50 ký tự.");
        
        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithErrorCode("Validation.MaxLength").WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        RuleFor(x => x.Email)
            .MaximumLength(100).WithErrorCode("Validation.MaxLength").WithMessage("Email không được vượt quá 100 ký tự.");

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000).WithErrorCode("Validation.MaxLength").WithMessage("Ghi chú không được vượt quá 1000 ký tự.");

        // RuleFor(x => x.HopDongs)
        //     .NotEmpty().WithErrorCode("DoiTac.HopDongsNotEmpty").WithMessage("Phải có ít nhất một hợp đồng cung cấp dịch vụ.");

        RuleForEach(x => x.HopDongs).ChildRules(hopDong =>
        {
            hopDong.RuleFor(x => x.SoHopDong).NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Số hợp đồng không được để trống.");
            hopDong.RuleFor(x => x.NgayHetHan)
                .GreaterThan(x => x.NgayKy).WithErrorCode("DoiTac.NgayHetHanInvalid").WithMessage("Ngày hết hạn phải sau ngày ký.");
            hopDong.RuleFor(x => x.GiaTri).GreaterThanOrEqualTo(0).WithErrorCode("DoiTac.GiaTriHopDongNegative").WithMessage("Giá trị hợp đồng không được âm.");
            
            // Validate Service Fields directly on HopDongRequestDto
            hopDong.RuleFor(x => x.MaDichVu).NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Mã dịch vụ không được để trống.");
            hopDong.RuleFor(x => x.TenDichVu).NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Tên dịch vụ không được để trống.");
            hopDong.RuleFor(x => x.LoaiDichVuId).GreaterThan(0).WithErrorCode("DoiTac.LoaiDichVuInvalid").WithMessage("Loại dịch vụ không hợp lệ.");
            hopDong.RuleFor(x => x.DonViTinh).NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("Đơn vị tính không được để trống.");
            hopDong.RuleFor(x => x.SoLuongToiDa).GreaterThan(0).When(x => x.SoLuongToiDa != null).WithErrorCode("DoiTac.SoLuongToiDaInvalid").WithMessage("Số lượng tối đa phải lớn hơn 0.");
        });
    }
}
