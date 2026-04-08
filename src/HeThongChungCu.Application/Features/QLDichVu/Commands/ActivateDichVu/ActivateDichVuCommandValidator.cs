namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateDichVu;

public class ActivateDichVuCommandValidator : AbstractValidator<ActivateDichVuCommand>
{
    public ActivateDichVuCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(ids => ids.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}