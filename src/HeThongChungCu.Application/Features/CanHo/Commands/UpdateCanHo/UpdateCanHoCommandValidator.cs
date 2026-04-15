using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandValidator : AbstractValidator<UpdateCanHoCommand>
{
    public UpdateCanHoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Giá trị ID phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TangId)
            .GreaterThan(0).WithMessage("Giá trị Tầng phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.MaCanHo)
            .NotEmpty().WithMessage("Mã căn hộ không được để trống.")
            .MaximumLength(20).WithMessage("Mã căn hộ không được vượt quá 20 ký tự.");

        RuleFor(x => x.TenCanHo)
            .NotEmpty().WithMessage("Tên căn hộ không được để trống.")
            .MaximumLength(100).WithMessage("Tên căn hộ không được vượt quá 100 ký tự.");

        RuleFor(x => x.DienTich)
            .GreaterThan(0).WithMessage("Giá trị Diện tích phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.SoPhongNgu)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị Số phòng ngủ phải nằm trong khoảng từ 0 đến 2147483647.");

        RuleFor(x => x.SoPhongTam)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị Số phòng tắm phải nằm trong khoảng từ 0 đến 2147483647.");

        RuleFor(x => x.LoaiCanHoId)
            .Must(id => LoaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại căn hộ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        RuleFor(x => x.TinhTrangCanHoId)
            .Must(id => TrangThaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage($"Tình trạng căn hộ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", TrangThaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
