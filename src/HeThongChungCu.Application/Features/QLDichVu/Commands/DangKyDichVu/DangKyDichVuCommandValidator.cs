using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DangKyDichVu;

public class DangKyDichVuCommandValidator : AbstractValidator<DangKyDichVuCommand>
{
    public DangKyDichVuCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("Mã căn hộ không được để trống.");

        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage("Không tìm thấy dịch vụ.");

        RuleFor(x => x.SoLuong)
            .GreaterThan(0).WithMessage("Giá trị Số lượng phải nằm trong khoảng từ 1 đến 2147483647.");
        
        RuleFor(x => x.NgaySuDung)
            .NotEmpty().WithMessage("Ngày sử dụng không được để trống.");
    }
}

