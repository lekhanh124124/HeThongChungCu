using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandValidator : AbstractValidator<TaoYeuCauPhuongTienCommand>
{
    public TaoYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("Giá trị Căn hộ phải nằm trong khoảng từ 1 đến 2147483647.")
            .GreaterThan(0).WithMessage("Giá trị Căn hộ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty().WithMessage("Loại yêu cầu không được để trống.")
            .Must(id => LoaiHanhDongYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiHanhDongYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        When(x => x.LoaiYeuCauId == LoaiHanhDongYeuCau.Them.Value, () => // AddVehicle
        {
            RuleFor(x => x.YeuCauLoaiPhuongTienId)
                .NotEmpty().WithMessage("Giá trị Loại xe phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Loại xe phải nằm trong khoảng từ 1 đến 2147483647.");

            RuleFor(x => x.YeuCauTenPhuongTien)
                .NotEmpty().WithMessage("Tên xe không được để trống.")
                .MaximumLength(100).WithMessage("Tên xe không được vượt quá 100 ký tự.");

            RuleFor(x => x.YeuCauBienSo)
                .NotEmpty().WithMessage("Biển số không được để trống.")
                .MaximumLength(20).WithMessage("Biển số không được vượt quá 20 ký tự.");

            RuleFor(x => x.YeuCauMauXe)
                .NotEmpty().WithMessage("Màu xe không được để trống.")
                .MaximumLength(50).WithMessage("Màu xe không được vượt quá 50 ký tự.");
        });

        When(x => x.LoaiYeuCauId == LoaiHanhDongYeuCau.Sua.Value || x.LoaiYeuCauId == LoaiHanhDongYeuCau.Xoa.Value, () => // Update/Delete
        {
            RuleFor(x => x.YeuCauPhuongTienId)
                .NotEmpty().WithMessage("Giá trị Yêu cầu phương tiện phải nằm trong khoảng từ 1 đến 2147483647.")
                .GreaterThan(0).WithMessage("Giá trị Yêu cầu phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");
        });

        When(x => x.FileIds != null, () =>
        {
            RuleForEach(x => x.FileIds)
                .GreaterThan(0).WithMessage("Giá trị Tệp tin phải nằm trong khoảng từ 1 đến 2147483647.");
        });
    }
}
