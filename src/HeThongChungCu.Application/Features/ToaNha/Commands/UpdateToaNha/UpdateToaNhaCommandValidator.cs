using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(255).WithMessage(ValidationErrors.MaxLength(255).Description);

        RuleFor(x => x.TrangThaiToaNhaId)
            .Must(id => TrangThaiToaNha.GetAll().Any(g => g.Value == id))
            .WithMessage(ToaNhaErrors.InvalidStatus(TrangThaiToaNha.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
