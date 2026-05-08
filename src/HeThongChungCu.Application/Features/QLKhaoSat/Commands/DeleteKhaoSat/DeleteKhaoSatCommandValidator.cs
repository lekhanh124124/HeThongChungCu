using FluentValidation;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.DeleteKhaoSat;

public class DeleteKhaoSatCommandValidator : AbstractValidator<DeleteKhaoSatCommand>
{
    public DeleteKhaoSatCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID đợt khảo sát không hợp lệ.");
    }
}
