namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandValidator : AbstractValidator<ThietLapCuTruCommand>
{
    public ThietLapCuTruCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .GreaterThan(0).WithMessage("ID căn hộ không hợp lệ.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("ID cư dân không hợp lệ.");

        RuleFor(x => x.LoaiQuanHeCuTruId)
            .Must(id => LoaiQuanHeCuTru.GetAll().Any(l => l.Value == id))
            .WithMessage($"Loại quan hệ cư trú không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", LoaiQuanHeCuTru.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
