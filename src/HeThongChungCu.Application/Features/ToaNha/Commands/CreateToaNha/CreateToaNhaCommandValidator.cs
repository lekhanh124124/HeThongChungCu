using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public class CreateToaNhaCommandValidator : AbstractValidator<CreateToaNhaCommand>
{
    public CreateToaNhaCommandValidator()
    {
        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage(ToaNhaErrors.MaToaNhaNotEmpty.Description)
            .MaximumLength(20).WithMessage(ToaNhaErrors.MaToaNhaMaxLength.Description);

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage(ToaNhaErrors.TenToaNhaNotEmpty.Description)
            .MaximumLength(100).WithMessage(ToaNhaErrors.TenToaNhaMaxLength.Description);

        RuleFor(x => x.Block)
            .NotEmpty().WithMessage("Block không được để trống.")
            .Length(1).WithMessage("Block phải là 1 ký tự.")
            .Must(b => char.IsLetter(b[0]) && char.IsUpper(b[0])).WithMessage("Block phải là một ký tự alphabet in hoa (A-Z).");


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage(ToaNhaErrors.DiaChiNotEmpty.Description)
            .MaximumLength(255).WithMessage(ToaNhaErrors.DiaChiMaxLength.Description);

    }
}
