using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteHoaDonDoiTac;

public class DeleteHoaDonDoiTacCommandValidator : AbstractValidator<DeleteHoaDonDoiTacCommand>
{
    public DeleteHoaDonDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithErrorCode("Validation.InvalidId").WithMessage("ID hóa đơn đối tác không hợp lệ.");
    }
}
