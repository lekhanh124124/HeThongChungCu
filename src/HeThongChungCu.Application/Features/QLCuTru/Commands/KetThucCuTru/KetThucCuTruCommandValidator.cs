namespace HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandValidator : AbstractValidator<KetThucCuTruCommand>
{
    public KetThucCuTruCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId)
            .GreaterThan(0).WithMessage("ID quan hệ cư trú không hợp lệ.");
    }
}
