using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatThongTinPhuongTien;

public sealed class CapNhatThongTinPhuongTienCommandValidator : AbstractValidator<CapNhatThongTinPhuongTienCommand>
{
    public CapNhatThongTinPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0).WithMessage("Giá trị Phương tiện phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TenPhuongTien)
            .NotEmpty().WithMessage("Tên xe không được để trống.")
            .MaximumLength(100).WithMessage("Tên xe không được vượt quá 100 ký tự.");

        RuleFor(x => x.LoaiPhuongTienId)
            .Must(id => LoaiPhuongTien.GetAll().Any(l => l.Value == id))
            .WithMessage($"Loại phương tiện không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiPhuongTien.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        RuleFor(x => x.BienSo)
            .NotEmpty().WithMessage("Biển số không được để trống.")
            .MaximumLength(20).WithMessage("Biển số không được vượt quá 20 ký tự.");

        RuleFor(x => x.MauXe)
            .NotEmpty().WithMessage("Màu xe không được để trống.")
            .MaximumLength(50).WithMessage("Màu xe không được vượt quá 50 ký tự.");
    }
}
