using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;

public class CapNhatYeuCauCuTruCommandValidator : AbstractValidator<CapNhatYeuCauCuTruCommand>
{
    public CapNhatYeuCauCuTruCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationErrors.NotEmpty.Description);

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .WithMessage(ValidationErrors.MaxLength(50).Description);
            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .WithMessage(ValidationErrors.MaxLength(50).Description);
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage(ValidationErrors.MaxLength(20).Description);
            RuleFor(x => x.CCCD)
                .MaximumLength(50)
                .WithMessage(ValidationErrors.MaxLength(50).Description);
            RuleFor(x => x.DiaChi)
                .MaximumLength(200)
                .WithMessage(ValidationErrors.MaxLength(200).Description);
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
        });
    }
}
