using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public class CreateYeuCauSuaChuaCommandValidator : AbstractValidator<CreateYeuCauSuaChuaCommand>
{
    public CreateYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("Căn hộ không được để trống.")
            .GreaterThan(0).WithMessage("ID Căn hộ không hợp lệ.");

        RuleFor(x => x.PhamViId)
            .NotEmpty().WithMessage("Phạm vi sửa chữa không được để trống.")
            .Must(id => PhamViSuaChua.GetAll().Any(v => v.Value == id))
            .WithMessage(x => $"Phạm vi sửa chữa không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", PhamViSuaChua.GetAll().Select(v => $"{v.Value} ({v.Name})"))}.");

        RuleFor(x => x.LoaiSuCoId)
            .NotEmpty().WithMessage("Loại sự cố không được để trống.")
            .Must(id => LoaiSuCoKyThuat.GetAll().Any(v => v.Value == id))
            .WithMessage(x => $"Loại sự cố không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiSuCoKyThuat.GetAll().Select(v => $"{v.Value} ({v.Name})"))}.");


        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung yêu cầu không được để trống.")
            .MaximumLength(1000).WithMessage("Nội dung yêu cầu không được vượt quá 1000 ký tự.");

        RuleFor(x => x.MoTaViTri)
            .MaximumLength(500).WithMessage("Mô tả vị trí không được vượt quá 500 ký tự.");

        When(x => x.DanhSachTepIds != null, () =>
        {
            RuleForEach(x => x.DanhSachTepIds)
                .GreaterThan(0).WithMessage("ID Tệp tin không hợp lệ.");
        });
    }
}
