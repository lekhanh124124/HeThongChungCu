using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public class UpdateDichVuCommandValidator : AbstractValidator<UpdateDichVuCommand>
{
    public UpdateDichVuCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Giá trị Dịch vụ phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}

