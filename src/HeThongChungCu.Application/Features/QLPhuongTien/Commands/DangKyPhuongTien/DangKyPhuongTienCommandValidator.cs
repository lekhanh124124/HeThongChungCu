using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.DangKyPhuongTien;

public sealed class DangKyPhuongTienCommandValidator : AbstractValidator<DangKyPhuongTienCommand>
{
    public DangKyPhuongTienCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .GreaterThan(0)
            .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);

        RuleFor(x => x.TenPhuongTien)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(100)
            .WithMessage(ValidationErrors.MaxLength(100).Description);

        RuleFor(x => x.LoaiPhuongTienId)
            .Must(id => LoaiPhuongTien.GetAll().Any(l => l.Value == id))
            .WithMessage(PhuongTienErrors.InvalidType(LoaiPhuongTien.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        RuleFor(x => x.BienSo)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(20)
            .WithMessage(ValidationErrors.MaxLength(20).Description);

        RuleFor(x => x.MauXe)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50)
            .WithMessage(ValidationErrors.MaxLength(50).Description);

        RuleForEach(x => x.HinhAnhIds)
            .GreaterThan(0)
            .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
    }
}
