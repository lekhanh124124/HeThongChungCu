using FluentValidation;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

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

        RuleFor(x => x.Block)
            .NotEmpty().WithMessage("Block không được để trống.")
            .Length(1).WithMessage("Block phải là 1 ký tự.")
            .Must(b => char.IsLetter(b[0]) && char.IsUpper(b[0])).WithMessage("Block phải là một ký tự alphabet in hoa (A-Z).");


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Địa chỉ không được để trống.")
            .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự.");

    }
}
