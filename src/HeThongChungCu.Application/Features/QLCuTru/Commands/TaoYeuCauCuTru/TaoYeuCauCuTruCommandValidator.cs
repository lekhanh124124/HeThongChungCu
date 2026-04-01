using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandValidator : AbstractValidator<TaoYeuCauCuTruCommand>
{
    public TaoYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description);

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description)
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage(YeuCauCuTruErrors.InvalidType(LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);


        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddMember
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
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage(ValidationErrors.MaxLength(20).Description);
            RuleFor(x => x.Dob)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
            RuleFor(x => x.GioiTinhId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
            RuleFor(x => x.CCCD)
                .MaximumLength(50)
                .WithMessage(ValidationErrors.MaxLength(50).Description);
            RuleFor(x => x.DiaChi)
                .MaximumLength(200)
                .WithMessage(ValidationErrors.MaxLength(200).Description);
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
        });

        When(x => x.LoaiYeuCauId != LoaiYeuCau.Them.Value, () => // Update/Remove/ChangeHead
        {
            RuleFor(x => x.TargetQuanHeCuTruId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value, () => // UpdateRelationship
        {
            RuleFor(x => x.LoaiQuanHeId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
            attachment.RuleFor(a => a.SoGiayTo)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description)
                .MaximumLength(100)
                .WithMessage(ValidationErrors.MaxLength(100).Description);
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty()
                .WithMessage(ValidationErrors.NotEmpty.Description);
            attachment.RuleForEach(a => a.FileIds)
                .GreaterThan(0)
                .WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
        });
    }
}
