using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandValidator : AbstractValidator<ChinhSuaHoSoCommand>
{
    public ChinhSuaHoSoCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);
        RuleFor(x => x.Dob).NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description);
        RuleFor(x => x.GioiTinhId).InclusiveBetween(1, 2).WithMessage(ValidationErrors.Range(1, 2).Description);
        RuleFor(x => x.DiaChi).MaximumLength(500).WithMessage(ValidationErrors.MaxLength(500).Description);
        RuleFor(x => x.LoaiQuanHeCuTruId).GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
    }
}
