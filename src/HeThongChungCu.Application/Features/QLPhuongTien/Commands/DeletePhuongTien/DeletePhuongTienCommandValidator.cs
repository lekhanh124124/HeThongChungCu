using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.DeletePhuongTien;

public class DeletePhuongTienCommandValidator : AbstractValidator<DeletePhuongTienCommand>
{
    public DeletePhuongTienCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.");
    }
}
