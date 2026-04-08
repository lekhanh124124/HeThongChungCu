using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeBangGia;

public class RevokeBangGiaCommandValidator : AbstractValidator<RevokeBangGiaCommand>
{
    public RevokeBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .GreaterThan(0).WithMessage("ID dịch vụ không hợp lệ.");

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID cần thu hồi không được để trống.");
    }
}
