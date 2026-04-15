using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandValidator : AbstractValidator<ThietLapCuTruCommand>
{
    public ThietLapCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .GreaterThan(0).WithMessage("Giá trị ID phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Giá trị Người dùng phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.LoaiQuanHeCuTruId)
            .Must(id => LoaiQuanHeCuTru.GetAll().Any(l => l.Value == id))
            .WithMessage($"Loại quan hệ cư trú không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", LoaiQuanHeCuTru.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

    }
}
