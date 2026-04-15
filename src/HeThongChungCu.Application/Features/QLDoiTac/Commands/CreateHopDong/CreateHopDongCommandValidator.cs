using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHopDong;

public class CreateHopDongCommandValidator : AbstractValidator<CreateHopDongCommand>
{
    public CreateHopDongCommandValidator()
    {
        RuleFor(x => x.DoiTacId)
            .NotEmpty().WithErrorCode("Validation.NotEmpty").WithMessage("ID không được để trống.");

        RuleFor(x => x.HopDong).ChildRules(hopDong =>
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
