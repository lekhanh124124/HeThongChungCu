using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandValidator : AbstractValidator<TaoHoSoCommand>
{
    public TaoHoSoCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50)
            .WithMessage(ValidationErrors.MaxLength(50).Description);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .MaximumLength(50)
            .WithMessage(ValidationErrors.MaxLength(50).Description);
        RuleFor(x => x.Dob)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .LessThan(DateTime.UtcNow)
            .WithMessage(ValidationErrors.DateInFuture.Description);
        RuleFor(x => x.GioiTinhId)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage(UserErrors.InvalidGender(GioiTinh.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
        RuleFor(x => x.DiaChi)
            .MaximumLength(200)
            .WithMessage(ValidationErrors.MaxLength(200).Description);

        RuleFor(x => x.TaiLieuCuTrus)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .When(x => x.TaiLieuCuTrus != null);

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(doc =>
        {
            doc.RuleFor(d => d.LoaiGiayToId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .Must(id => LoaiGiayTo.GetAll().Any(g => g.Value == id))
                .WithMessage(YeuCauCuTruErrors.InvalidDocumentType(LoaiGiayTo.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

            doc.RuleFor(d => d.SoGiayTo)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .MaximumLength(100)
                .WithMessage(ValidationErrors.MaxLength(100).Description);
            doc.RuleFor(d => d.FileIds)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
            doc.RuleForEach(d => d.FileIds)
                .GreaterThan(0)
                .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
        });
    }
}
