using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandValidator : AbstractValidator<TaoYeuCauPhuongTienCommand>
{
    public TaoYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.CanHoIdRange.Description)
            .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.CanHoIdRange.Description);

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty().WithMessage(YeuCauPhuongTienErrors.LoaiYeuCauNotEmpty.Description)
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage(YeuCauPhuongTienErrors.InvalidType(LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddVehicle
        {
            RuleFor(x => x.YeuCauLoaiPhuongTienId)
                .NotEmpty().WithMessage(YeuCauPhuongTienErrors.LoaiXeIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.LoaiXeIdRange.Description);

            RuleFor(x => x.YeuCauTenPhuongTien)
                .NotEmpty().WithMessage(YeuCauPhuongTienErrors.TenXeNotEmpty.Description)
                .MaximumLength(100).WithMessage(PhuongTienErrors.TenXeMaxLength.Description);

            RuleFor(x => x.YeuCauBienSo)
                .NotEmpty().WithMessage(YeuCauPhuongTienErrors.BienSoNotEmpty.Description)
                .MaximumLength(20).WithMessage(PhuongTienErrors.BienSoMaxLength.Description);

            RuleFor(x => x.YeuCauMauXe)
                .NotEmpty().WithMessage(YeuCauPhuongTienErrors.MauXeNotEmpty.Description)
                .MaximumLength(50).WithMessage(PhuongTienErrors.MauXeMaxLength.Description);
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value || x.LoaiYeuCauId == LoaiYeuCau.Xoa.Value, () => // Update/Delete
        {
            RuleFor(x => x.YeuCauPhuongTienId)
                .NotEmpty().WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.YeuCauPhuongTienIdRange.Description);
        });

        When(x => x.FileIds != null, () =>
        {
            RuleForEach(x => x.FileIds)
                .GreaterThan(0).WithMessage(YeuCauPhuongTienErrors.FileIdRange.Description);
        });
    }
}
