using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandValidator : AbstractValidator<ThietLapCuTruCommand>
{
    public ThietLapCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .GreaterThan(0).WithMessage(CanHoErrors.IdRange.Description);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage(UserErrors.UserIdRange.Description);

        RuleFor(x => x.LoaiQuanHeCuTruId)
            .Must(id => LoaiQuanHeCuTru.GetAll().Any(l => l.Value == id))
            .WithMessage(YeuCauCuTruErrors.InvalidRelationType(LoaiQuanHeCuTru.GetAll().Select(l => $"{l.Value} ({l.Name})")).Description);

    }
}
