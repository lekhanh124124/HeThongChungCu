using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;

public class CapNhatYeuCauCuTruCommandValidator : AbstractValidator<CapNhatYeuCauCuTruCommand>
{
    public CapNhatYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(YeuCauCuTruErrors.YeuCauCuTruIdRange.Description)
            .GreaterThan(0).WithMessage(YeuCauCuTruErrors.YeuCauCuTruIdRange.Description);

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage(UserErrors.FirstNameMaxLength.Description);
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage(UserErrors.LastNameMaxLength.Description);
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage(UserErrors.PhoneNumberMaxLength.Description);
            RuleFor(x => x.CCCD)
                .MaximumLength(50).WithMessage(UserErrors.CCCDMaxLength.Description);
            RuleFor(x => x.DiaChi)
                .MaximumLength(200).WithMessage(UserErrors.DiaChiMaxLength.Description);
        });

        RuleForEach(x => x.TaiLieuCuTrus).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.LoaiGiayToId)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.GiayToIdRange.Description)
                .GreaterThan(0).WithMessage(YeuCauCuTruErrors.GiayToIdRange.Description);
            attachment.RuleFor(a => a.SoGiayTo)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.SoGiayToNotEmpty.Description)
                .MaximumLength(100).WithMessage(YeuCauCuTruErrors.SoGiayToMaxLength.Description);
            attachment.RuleFor(a => a.FileIds)
                .NotEmpty().WithMessage(YeuCauCuTruErrors.FileIdsNotEmpty.Description);
        });
    }
}
