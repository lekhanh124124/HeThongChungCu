using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandValidator : AbstractValidator<TaoHoSoCommand>
{
    public TaoHoSoCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(UserErrors.FirstNameNotEmpty.Description)
            .MaximumLength(50).WithMessage(UserErrors.FirstNameMaxLength.Description);
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(UserErrors.LastNameNotEmpty.Description)
            .MaximumLength(50).WithMessage(UserErrors.LastNameMaxLength.Description);
        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage(UserErrors.DobNotEmpty.Description)
            .LessThan(DateTime.UtcNow).WithMessage(UserErrors.DobInFuture.Description);
        RuleFor(x => x.GioiTinhId)
            .NotEmpty().WithMessage(UserErrors.GenderNotEmpty.Description)
            .Must(id => GioiTinh.GetAll().Any(g => g.Value == id))
            .WithMessage(UserErrors.InvalidGender(GioiTinh.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);
        RuleFor(x => x.DiaChi)
            .MaximumLength(200).WithMessage(UserErrors.DiaChiMaxLength.Description);

        RuleFor(x => x.TaiLieuCuTrus)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.FileIdsNotEmpty.Description)
            .When(x => x.TaiLieuCuTrus != null);

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(doc =>
        {
            doc.RuleFor(d => d.LoaiGiayToId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.GiayToIdRange.Description)
                .Must(id => LoaiGiayTo.GetAll().Any(g => g.Value == id))
                .WithMessage(YeuCauCuTruErrors.InvalidDocumentType(LoaiGiayTo.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

            doc.RuleFor(d => d.SoGiayTo)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.SoGiayToNotEmpty.Description)
                .MaximumLength(100).WithMessage(YeuCauCuTruErrors.SoGiayToMaxLength.Description);
            doc.RuleFor(d => d.FileIds)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.FileIdsNotEmpty.Description);
            doc.RuleForEach(d => d.FileIds)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.FileIdRange.Description);
        });
    }
}
