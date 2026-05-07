using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CapNhatTienDoBaoTri;

public class CapNhatTienDoBaoTriCommandValidator : AbstractValidator<CapNhatTienDoBaoTriCommand>
{
    public CapNhatTienDoBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID phiếu bảo trì không được để trống.");

        RuleFor(x => x.Checklists)
            .NotNull().WithMessage("Danh sách checklist cập nhật không được null.");

        RuleForEach(x => x.Checklists).ChildRules(update =>
        {
            update.RuleFor(u => u.ChecklistId)
                .NotEmpty().WithMessage("ID checklist không được để trống.");
        });

        RuleForEach(x => x.VatTus).ChildRules(vattu =>
        {
            vattu.RuleFor(v => v.TenVatTu)
                .NotEmpty().WithMessage("Tên vật tư không được để trống.")
                .MaximumLength(200).WithMessage("Tên vật tư không vượt quá 200 ký tự.");

            vattu.RuleFor(v => v.SoLuong)
                .GreaterThan(0).WithMessage("Số lượng vật tư phải lớn hơn 0.");

            vattu.RuleFor(v => v.DonGia)
                .GreaterThanOrEqualTo(0).WithMessage("Đơn giá vật tư phải lớn hơn hoặc bằng 0.");
        });
    }
}
