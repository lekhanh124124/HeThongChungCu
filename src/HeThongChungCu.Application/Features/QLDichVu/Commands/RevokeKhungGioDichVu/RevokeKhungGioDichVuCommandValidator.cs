using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeKhungGioDichVu;

public class RevokeKhungGioDichVuCommandValidator : AbstractValidator<RevokeKhungGioDichVuCommand>
{
    public RevokeKhungGioDichVuCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .GreaterThan(0).WithMessage("ID dịch vụ không hợp lệ.");

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID khung giờ cần thu hồi không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
