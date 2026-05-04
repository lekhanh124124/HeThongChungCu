using FluentValidation;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.DeleteChiSoTieuThu;

public class DeleteChiSoTieuThuCommandValidator : AbstractValidator<DeleteChiSoTieuThuCommand>
{
    public DeleteChiSoTieuThuCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID cần xóa không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("ID không hợp lệ.");
    }
}
