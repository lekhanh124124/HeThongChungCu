using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandValidator : AbstractValidator<CreateTangCommand>
{
    public CreateTangCommandValidator()
    {
        RuleFor(x => x.MaTang)
            .NotEmpty().WithMessage(TangErrors.MaTangNotEmpty.Description)
            .MaximumLength(20).WithMessage(TangErrors.MaTangMaxLength.Description);

        RuleFor(x => x.TenTang)
            .NotEmpty().WithMessage(TangErrors.TenTangNotEmpty.Description)
            .MaximumLength(100).WithMessage(TangErrors.TenTangMaxLength.Description);

        RuleFor(x => x.ToaNhaId)
            .GreaterThan(0).WithMessage(ToaNhaErrors.ToaNhaIdRange.Description);

        RuleFor(x => x.LoaiTangId)
            .Must(id => LoaiTang.GetAll().Any(g => g.Value == id))
            .WithMessage(TangErrors.InvalidType(LoaiTang.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
