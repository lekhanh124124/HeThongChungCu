using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandValidator : AbstractValidator<ChinhSuaHoSoCommand>
{
    public ChinhSuaHoSoCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dob).NotEmpty();
        RuleFor(x => x.GioiTinhId).InclusiveBetween(1, 2);
        RuleFor(x => x.DiaChi).MaximumLength(500);
        RuleFor(x => x.LoaiQuanHeCuTruId).GreaterThan(0);
    }
}
