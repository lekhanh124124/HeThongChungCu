using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatTrangThaiPhuongTien;

public sealed class CapNhatTrangThaiPhuongTienCommandValidator : AbstractValidator<CapNhatTrangThaiPhuongTienCommand>
{
    public CapNhatTrangThaiPhuongTienCommandValidator()
    {
        RuleFor(x => x.PhuongTienIds)
            .NotEmpty()
            .WithMessage("Danh sách phương tiện không được để trống.");

        RuleForEach(x => x.PhuongTienIds)
            .GreaterThan(0)
            .WithMessage("Id phương tiện không hợp lệ.");

        RuleFor(x => x.TrangThaiPhuongTienId)
            .Must(id => TrangThaiPhuongTien.GetAll().Any(l => l.Value == id))
            .WithMessage($"Trạng thái không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", TrangThaiPhuongTien.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
