using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatThongTinPhuongTien;

public sealed class CapNhatThongTinPhuongTienCommandValidator : AbstractValidator<CapNhatThongTinPhuongTienCommand>
{
    public CapNhatThongTinPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienId)
            .GreaterThan(0).WithMessage(PhuongTienErrors.PhuongTienIdRange.Description);

        RuleFor(x => x.TenPhuongTien)
            .NotEmpty().WithMessage(PhuongTienErrors.TenXeNotEmpty.Description)
            .MaximumLength(100).WithMessage(PhuongTienErrors.TenXeMaxLength.Description);

        RuleFor(x => x.LoaiPhuongTienId)
            .Must(id => LoaiPhuongTien.GetAll().Any(l => l.Value == id))
            .WithMessage(PhuongTienErrors.InvalidType(LoaiPhuongTien.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        RuleFor(x => x.BienSo)
            .NotEmpty().WithMessage(PhuongTienErrors.BienSoNotEmpty.Description)
            .MaximumLength(20).WithMessage(PhuongTienErrors.BienSoMaxLength.Description);

        RuleFor(x => x.MauXe)
            .NotEmpty().WithMessage(PhuongTienErrors.MauXeNotEmpty.Description)
            .MaximumLength(50).WithMessage(PhuongTienErrors.MauXeMaxLength.Description);
    }
}
