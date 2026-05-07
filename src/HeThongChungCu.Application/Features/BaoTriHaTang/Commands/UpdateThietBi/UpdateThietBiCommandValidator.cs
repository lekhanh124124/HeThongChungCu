namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateThietBi;

public class UpdateThietBiCommandValidator : AbstractValidator<UpdateThietBiCommand>
{
    public UpdateThietBiCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID thiết bị không hợp lệ.");

        RuleFor(x => x.TenThietBi)
            .NotEmpty().WithMessage("Tên thiết bị không được để trống.")
            .MaximumLength(200).WithMessage("Tên thiết bị không vượt quá 200 ký tự.");

        RuleFor(x => x.LoaiThietBi)
            .NotEmpty().WithMessage("Loại thiết bị không được để trống.")
            .MaximumLength(100).WithMessage("Loại thiết bị không vượt quá 100 ký tự.");

        RuleFor(x => x.ViTri)
            .NotEmpty().WithMessage("Vị trí không được để trống.")
            .MaximumLength(250).WithMessage("Vị trí không vượt quá 250 ký tự.");

        RuleFor(x => x.NgayMua)
            .NotEmpty().WithMessage("Ngày mua không được để trống.");

        RuleFor(x => x.GiaTriBanDau)
            .GreaterThanOrEqualTo(0).When(x => x.GiaTriBanDau.HasValue)
            .WithMessage("Giá trị ban đầu không được nhỏ hơn 0.");
    }
}
