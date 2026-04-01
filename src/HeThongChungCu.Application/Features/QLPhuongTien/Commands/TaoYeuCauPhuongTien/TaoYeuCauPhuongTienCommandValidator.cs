using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandValidator : AbstractValidator<TaoYeuCauPhuongTienCommand>
{
    public TaoYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description);

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage(YeuCauPhuongTienErrors.InvalidType(LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddVehicle
        {
            RuleFor(x => x.YeuCauLoaiPhuongTienId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);

            RuleFor(x => x.YeuCauTenPhuongTien)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .MaximumLength(100)
                .WithMessage(ValidationErrors.MaxLength(100).Description);

            RuleFor(x => x.YeuCauBienSo)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .MaximumLength(20)
                .WithMessage(ValidationErrors.MaxLength(20).Description);

            RuleFor(x => x.YeuCauMauXe)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .MaximumLength(50)
                .WithMessage(ValidationErrors.MaxLength(50).Description);
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value || x.LoaiYeuCauId == LoaiYeuCau.Xoa.Value, () => // Update/Delete
        {
            RuleFor(x => x.YeuCauPhuongTienId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
        });

        When(x => x.FileIds != null, () =>
        {
            RuleForEach(x => x.FileIds)
                .GreaterThan(0)
                .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
        });
    }
}
