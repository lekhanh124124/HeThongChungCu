using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.XacNhanThanhToanDoiTac;

public class XacNhanThanhToanDoiTacCommandValidator : AbstractValidator<XacNhanThanhToanDoiTacCommand>
{
    public XacNhanThanhToanDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithErrorCode("Validation.InvalidId").WithMessage("ID hóa đơn đối tác không hợp lệ.");
    }
}
