using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ToaNhaErrors.ToaNhaIdRange.Description);

        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage(ToaNhaErrors.MaToaNhaNotEmpty.Description)
            .MaximumLength(20).WithMessage(ToaNhaErrors.MaToaNhaMaxLength.Description);

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage(ToaNhaErrors.TenToaNhaNotEmpty.Description)
            .MaximumLength(100).WithMessage(ToaNhaErrors.TenToaNhaMaxLength.Description);


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage(ToaNhaErrors.DiaChiNotEmpty.Description)
            .MaximumLength(255).WithMessage(ToaNhaErrors.DiaChiMaxLength.Description);

        RuleFor(x => x.TrangThaiToaNhaId)
            .Must(id => TrangThaiToaNha.GetAll().Any(g => g.Value == id))
            .WithMessage(ToaNhaErrors.InvalidStatus(TrangThaiToaNha.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
