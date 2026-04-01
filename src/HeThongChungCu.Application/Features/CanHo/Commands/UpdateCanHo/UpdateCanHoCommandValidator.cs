using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public class UpdateCanHoCommandValidator : AbstractValidator<UpdateCanHoCommand>
{
    public UpdateCanHoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.TangId)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.MaCanHo)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20).WithMessage(ValidationErrors.MaxLength(20).Description);

        RuleFor(x => x.TenCanHo)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100).WithMessage(ValidationErrors.MaxLength(100).Description);

        RuleFor(x => x.DienTich)
            .GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.SoPhongNgu)
            .GreaterThanOrEqualTo(0).WithMessage(ValidationErrors.Range(0, int.MaxValue).Description);

        RuleFor(x => x.SoPhongTam)
            .GreaterThanOrEqualTo(0).WithMessage(ValidationErrors.Range(0, int.MaxValue).Description);

        RuleFor(x => x.LoaiCanHoId)
            .Must(id => LoaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage(CanHoErrors.InvalidType(LoaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        RuleFor(x => x.TinhTrangCanHoId)
            .Must(id => TrangThaiCanHo.GetAll().Any(g => g.Value == id))
            .WithMessage(CanHoErrors.InvalidStatus(TrangThaiCanHo.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
    }
}
