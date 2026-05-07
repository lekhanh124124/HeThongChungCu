using FluentValidation;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteThietBi;

public class DeleteThietBiCommandValidator : AbstractValidator<DeleteThietBiCommand>
{
    public DeleteThietBiCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID thiết bị không được để trống.");
    }
}
