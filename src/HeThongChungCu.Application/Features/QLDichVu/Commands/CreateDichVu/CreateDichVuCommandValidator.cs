using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateDichVu;

public class CreateDichVuCommandValidator : AbstractValidator<CreateDichVuCommand>
{
    public CreateDichVuCommandValidator()
    {
        RuleFor(x => x.MaDichVu)
            .NotEmpty().WithMessage("Mã dịch vụ không được để trống.")
            .MaximumLength(20).WithMessage("Mã dịch vụ không được vượt quá 20 ký tự.");

        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage("Tên dịch vụ không được để trống.")
            .MaximumLength(200).WithMessage("Tên dịch vụ không được vượt quá 200 ký tự.");

        RuleFor(x => x.LoaiDichVuId)
            .GreaterThan(0).WithMessage($"Loại dịch vụ không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiDichVu.GetAll().Select(x => x.Name))}.");

        RuleFor(x => x.DonViTinh)
            .NotEmpty().WithMessage("Đơn vị tính không được để trống.")
            .MaximumLength(50).WithMessage("Đơn vị tính không được vượt quá 50 ký tự.");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
    }
}

