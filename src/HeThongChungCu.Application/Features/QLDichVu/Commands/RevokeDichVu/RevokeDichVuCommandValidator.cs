namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeDichVu;

public class RevokeDichVuCommandValidator : AbstractValidator<RevokeDichVuCommand>
{
    public RevokeDichVuCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
