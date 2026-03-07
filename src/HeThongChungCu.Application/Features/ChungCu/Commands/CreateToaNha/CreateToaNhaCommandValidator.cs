using HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;

public class CreateToaNhaCommandValidator : AbstractValidator<CreateToaNhaCommand>
{
    public CreateToaNhaCommandValidator()
    {
        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage("Mã tòa nhà không được để trống.")
            .MaximumLength(20).WithMessage("Mã tòa nhà không được vượt quá 20 ký tự.");

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage("Tên tòa nhà không được để trống.")
            .MaximumLength(100).WithMessage("Tên tòa nhà không được vượt quá 100 ký tự.");

        RuleFor(x => x.SoTang)
            .GreaterThan(0).WithMessage("Số tầng phải lớn hơn 0.");
    }
}
