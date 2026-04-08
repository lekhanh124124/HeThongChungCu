using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteDoiTac;

public class DeleteDoiTacsCommandValidator : AbstractValidator<DeleteDoiTacsCommand>
{
    public DeleteDoiTacsCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.");
    }
}
