namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatQuanHe;

public class CapNhatQuanHeCommandValidator : AbstractValidator<CapNhatQuanHeCommand>
{
    public CapNhatQuanHeCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId)
            .GreaterThan(0).WithMessage("ID quan hệ cư trú không hợp lệ.");

        RuleFor(x => x.LoaiQuanHeCuTruId)
            .Must(id => LoaiQuanHeCuTru.GetAll().Any(l => l.Value == id))
            .WithMessage($"Loại quan hệ cư trú không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", LoaiQuanHeCuTru.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
