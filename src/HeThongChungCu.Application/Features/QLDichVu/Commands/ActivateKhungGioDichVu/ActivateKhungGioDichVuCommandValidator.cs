using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateKhungGioDichVu;

public class ActivateKhungGioDichVuCommandValidator : AbstractValidator<ActivateKhungGioDichVuCommand>
{
    public ActivateKhungGioDichVuCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .GreaterThan(0)
            .WithErrorCode("Validation.Range")
            .WithMessage("Giá trị Dịch vụ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
