using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public class UpdateDichVuCommandValidator : AbstractValidator<UpdateDichVuCommand>
{
    public UpdateDichVuCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Giá trị Dịch vụ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage("Tên dịch vụ không được để trống.")
            .MaximumLength(255).WithMessage("Tên dịch vụ không được vượt quá 255 ký tự.");

        RuleFor(x => x.LoaiDichVuId)
            .NotEmpty().WithMessage("Loại dịch vụ không được để trống.")
            .Must(id => LoaiDichVu.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại dịch vụ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiDichVu.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage("Đơn vị tính không được để trống.")
            .MaximumLength(50).WithMessage("Đơn vị tính không được vượt quá 50 ký tự.");

        RuleFor(x => x.MoTa)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.SoLuongToiDa)
            .GreaterThan(0).When(x => x.SoLuongToiDa.HasValue)
            .WithMessage("Số lượng tối đa phải lớn hơn 0.");
    }
}

