using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateBangGia;

public class ActivateBangGiaCommandValidator : AbstractValidator<ActivateBangGiaCommand>
{
    public ActivateBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .GreaterThan(0).WithMessage("DichVuId không hợp lệ");

        RuleFor(x => x.Ids)
            .NotNull().WithMessage("Danh sách BangGiaId không được để trống")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("BangGiaId không hợp lệ");
    }
}
