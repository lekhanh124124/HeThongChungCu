using FluentValidation;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public class UpdateDichVuCommandValidator : AbstractValidator<UpdateDichVuCommand>
{
    public UpdateDichVuCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(DichVuErrors.DichVuIdRange.Description);
    }
}

