using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandValidator : AbstractValidator<ChinhSuaHoSoCommand>
{
    public ChinhSuaHoSoCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0).WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description);
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(UserErrors.FirstNameNotEmpty.Description)
            .MaximumLength(100).WithMessage(UserErrors.FirstNameMaxLength.Description);
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(UserErrors.LastNameNotEmpty.Description)
            .MaximumLength(100).WithMessage(UserErrors.LastNameMaxLength.Description);
        RuleFor(x => x.Dob).NotEmpty().WithMessage(UserErrors.DobNotEmpty.Description);
        RuleFor(x => x.GioiTinhId).InclusiveBetween(1, 2).WithMessage(UserErrors.GenderRange.Description);
        RuleFor(x => x.DiaChi).MaximumLength(500).WithMessage(UserErrors.DiaChiMaxLength.Description);
        RuleFor(x => x.LoaiQuanHeCuTruId).GreaterThan(0).WithMessage(YeuCauCuTruErrors.QuanHeIdRange.Description);
    }
}
