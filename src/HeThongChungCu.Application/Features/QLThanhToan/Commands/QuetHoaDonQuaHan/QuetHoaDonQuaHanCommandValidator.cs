using FluentValidation;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.QuetHoaDonQuaHan;

public class QuetHoaDonQuaHanCommandValidator : AbstractValidator<QuetHoaDonQuaHanCommand>
{
    public QuetHoaDonQuaHanCommandValidator()
    {
        // No validation rules needed as there are no command properties.
    }
}
