namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;

public class DeleteDichVuCommandValidator : AbstractValidator<DeleteDichVuCommand>
{
    public DeleteDichVuCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
