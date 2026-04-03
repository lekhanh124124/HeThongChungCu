using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public class CreateCanHoCommandValidator : AbstractValidator<CreateCanHoCommand>
{
    public CreateCanHoCommandValidator()
    {
        RuleFor(x => x.MaCanHo)
            .NotEmpty().WithMessage(CanHoErrors.MaCanHoNotEmpty.Description)
            .MaximumLength(20).WithMessage(CanHoErrors.MaCanHoMaxLength.Description);

        RuleFor(x => x.TenCanHo)
            .NotEmpty().WithMessage(CanHoErrors.TenCanHoNotEmpty.Description)
            .MaximumLength(100).WithMessage(CanHoErrors.TenCanHoMaxLength.Description);

        RuleFor(x => x.DienTich)
            .GreaterThan(0).WithMessage(CanHoErrors.DienTichRange.Description);

        RuleFor(x => x.TangId)
            .GreaterThan(0).WithMessage(CanHoErrors.TangRange.Description);

        RuleFor(x => x.SoPhongNgu)
            .GreaterThanOrEqualTo(0).WithMessage(CanHoErrors.SoPhongNguRange.Description);

        RuleFor(x => x.SoPhongTam)
            .GreaterThanOrEqualTo(0).WithMessage(CanHoErrors.SoPhongTamRange.Description);

        RuleFor(x => x.LoaiCanHoId)
            .Must(id => LoaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage(CanHoErrors.InvalidType(LoaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
