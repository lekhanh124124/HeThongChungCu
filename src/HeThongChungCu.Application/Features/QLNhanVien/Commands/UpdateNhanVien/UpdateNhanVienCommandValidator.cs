using FluentValidation;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.UpdateNhanVien;

public class UpdateNhanVienCommandValidator : AbstractValidator<UpdateNhanVienCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public UpdateNhanVienCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(x => x.Id)
            .NotEmpty();

        // User Profile Validation
        RuleFor(x => x.Ho)
            .NotEmpty().WithMessage("Họ không được để trống.")
            .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự.");

        RuleFor(x => x.Ten)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự.");

        RuleFor(x => x.NgaySinh)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .LessThan(_dateTimeProvider.UtcNow.DateTime).WithMessage("Ngày sinh không được là ngày trong tương lai.");

        RuleFor(x => x.GioiTinhId)
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage($"Giới tính không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", GioiTinh.GetAll().Select(g => g.Name))}.");

        RuleFor(x => x.CCCD)
            .MaximumLength(50).WithMessage("CCCD/CMND không được vượt quá 50 ký tự.");

        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        // Staff Validation
        RuleFor(x => x.LoaiNhanVienId)
            .Must(id => LoaiNhanVien.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại nhân viên không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiNhanVien.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        RuleFor(x => x.TrangThaiNhanVienId)
            .Must(id => TrangThaiNhanVien.GetAll().Any(s => s.Value == id))
            .WithMessage($"Trạng thái nhân viên không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", TrangThaiNhanVien.GetAll().Select(t => t.Name))}.");

        RuleFor(x => x.NgayVaoLam)
            .NotEmpty();
    }
}
