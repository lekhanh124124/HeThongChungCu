using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public class CreateToaNhaCommandValidator : AbstractValidator<CreateToaNhaCommand>
{
    public CreateToaNhaCommandValidator()
    {
        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(255).WithMessage(ValidationErrors.MaxLength(255).Description);

    }
}
