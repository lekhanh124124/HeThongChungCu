using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandValidator : AbstractValidator<CreateTangCommand>
{
    public CreateTangCommandValidator()
    {
        RuleFor(x => x.MaTang)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);

        RuleFor(x => x.TenTang)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);

        RuleFor(x => x.ToaNhaId)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.LoaiTangId)
            .Must(id => LoaiTang.GetAll().Any(g => g.Value == id))
            .WithMessage(TangErrors.InvalidType(LoaiTang.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
