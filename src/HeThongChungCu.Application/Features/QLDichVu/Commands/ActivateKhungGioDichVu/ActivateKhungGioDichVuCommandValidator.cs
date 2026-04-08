using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateKhungGioDichVu;

public class ActivateKhungGioDichVuCommandValidator : AbstractValidator<ActivateKhungGioDichVuCommand>
{
    public ActivateKhungGioDichVuCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .GreaterThan(0)
            .WithErrorCode(DichVuErrors.DichVuIdRange.Code)
            .WithMessage(DichVuErrors.DichVuIdRange.Description);

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
