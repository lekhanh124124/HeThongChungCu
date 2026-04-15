using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandValidator : AbstractValidator<KetThucCuTruCommand>
{
    public KetThucCuTruCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId)
            .GreaterThan(0).WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
