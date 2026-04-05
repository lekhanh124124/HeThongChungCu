using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandValidator : AbstractValidator<TaoYeuCauCuTruCommand>
{
    public TaoYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.CanHoIdRange.Description)
            .GreaterThan(0).WithMessage(YeuCauCuTruErrors.CanHoIdRange.Description);

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.LoaiYeuCauNotEmpty.Description)
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage(YeuCauCuTruErrors.InvalidType(LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);


        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddMember
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(UserErrors.FirstNameNotEmpty.Description)
                .MaximumLength(50).WithMessage(UserErrors.FirstNameMaxLength.Description);
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(UserErrors.LastNameNotEmpty.Description)
                .MaximumLength(50).WithMessage(UserErrors.LastNameMaxLength.Description);
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage(UserErrors.PhoneNumberMaxLength.Description);
            RuleFor(x => x.Dob)
                .NotEmpty().WithMessage(UserErrors.DobNotEmpty.Description);
            RuleFor(x => x.GioiTinhId)
                .NotEmpty().WithMessage(UserErrors.GenderNotEmpty.Description);
            RuleFor(x => x.CCCD)
                .MaximumLength(50).WithMessage(UserErrors.CCCDMaxLength.Description);
            RuleFor(x => x.DiaChi)
                .MaximumLength(200).WithMessage(UserErrors.DiaChiMaxLength.Description);
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description);
        });

        When(x => x.LoaiYeuCauId != LoaiYeuCau.Them.Value, () => // Update/Remove/ChangeHead
        {
            RuleFor(x => x.TargetQuanHeCuTruId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description);
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value, () => // UpdateRelationship
        {
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description);
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.GiayToIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.GiayToIdRange.Description);
            //attachment.RuleFor(a => a.SoGiayTo)
            //    .NotEmpty().WithMessage(YeuCauCuTruErrors.SoGiayToNotEmpty.Description)
            //    .MaximumLength(100).WithMessage(YeuCauCuTruErrors.SoGiayToMaxLength.Description);
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.FileIdsNotEmpty.Description);
            attachment.RuleForEach(a => a.FileIds)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.FileIdRange.Description);
        });
    }
}
