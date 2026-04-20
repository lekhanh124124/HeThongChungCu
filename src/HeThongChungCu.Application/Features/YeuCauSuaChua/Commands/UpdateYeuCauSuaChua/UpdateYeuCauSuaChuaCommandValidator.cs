using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.UpdateYeuCauSuaChua;

public class UpdateYeuCauSuaChuaCommandValidator : AbstractValidator<UpdateYeuCauSuaChuaCommand>
{
    public UpdateYeuCauSuaChuaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID yêu cầu không hợp lệ.");

        When(x => !x.IsWithdraw, () =>
        {
            When(x => x.PhamViId.HasValue, () =>
            {
                RuleFor(x => x.PhamViId!.Value)
                    .Must(id => PhamViSuaChua.GetAll().Any(v => v.Value == id))
                    .WithMessage(x => $"Phạm vi sửa chữa không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", PhamViSuaChua.GetAll().Select(v => $"{v.Value} ({v.Name})"))}.");
            });

            When(x => x.LoaiSuCoId.HasValue, () =>
            {
                RuleFor(x => x.LoaiSuCoId!.Value)
                    .Must(id => LoaiSuCoKyThuat.GetAll().Any(v => v.Value == id))
                    .WithMessage(x => $"Loại sự cố không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiSuCoKyThuat.GetAll().Select(v => $"{v.Value} ({v.Name})"))}.");
            });


            When(x => x.NoiDung != null, () =>
            {
                RuleFor(x => x.NoiDung)
                    .NotEmpty().WithMessage("Nội dung yêu cầu không được để trống.")
                    .MaximumLength(1000).WithMessage("Nội dung yêu cầu không được vượt quá 1000 ký tự.");
            });

            When(x => x.MoTaViTri != null, () =>
            {
                RuleFor(x => x.MoTaViTri)
                    .MaximumLength(500).WithMessage("Mô tả vị trí không được vượt quá 500 ký tự.");
            });

            When(x => x.DanhSachTepIds != null, () =>
            {
                RuleForEach(x => x.DanhSachTepIds)
                    .GreaterThan(0).WithMessage("ID Tệp tin không hợp lệ.");
            });
        });
    }
}
