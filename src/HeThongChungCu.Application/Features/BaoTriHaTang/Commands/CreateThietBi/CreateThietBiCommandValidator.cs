namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateThietBi;

public class CreateThietBiCommandValidator : AbstractValidator<CreateThietBiCommand>
{
    public CreateThietBiCommandValidator()
    {
        RuleFor(x => x.MaThietBi)
            .NotEmpty().WithMessage("Mã thiết bị không được để trống.")
            .MaximumLength(50).WithMessage("Mã thiết bị không vượt quá 50 ký tự.");

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
