using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public class TaoYeuCauPhuongTienCommandValidator : AbstractValidator<TaoYeuCauPhuongTienCommand>
{
    public TaoYeuCauPhuongTienCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty()
            .WithMessage("CanHoId không được để trống.");

        RuleFor(x => x.LoaiYeuCauId)
            .NotEmpty()
            .WithMessage("LoaiYeuCauId không được để trống.")
            .Must(id => LoaiYeuCau.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại yêu cầu không hợp lệ. Các giá trị hợp lệ: " +
                             $"{string.Join(", ", LoaiYeuCau.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Them.Value, () => // AddVehicle
        {
            RuleFor(x => x.YeuCauLoaiPhuongTienId)
                .NotEmpty()
                .WithMessage("Loại phương tiện không được để trống.");
            RuleFor(x => x.YeuCauTenPhuongTien)
                .NotEmpty()
                .WithMessage("Tên phương tiện không được để trống.")
                .MaximumLength(100)
                .WithMessage("Tên phương tiện không được vượt quá 100 ký tự.");
            RuleFor(x => x.YeuCauBienSo)
                .NotEmpty()
                .WithMessage("Biển số không được để trống.")
                .MaximumLength(20)
                .WithMessage("Biển số không được vượt quá 20 ký tự.");
            RuleFor(x => x.YeuCauMauXe)
                .NotEmpty()
                .WithMessage("Màu xe không được để trống.")
                .MaximumLength(50)
                .WithMessage("Màu xe không được vượt quá 50 ký tự.");
        });

        When(x => x.LoaiYeuCauId == LoaiYeuCau.Sua.Value || x.LoaiYeuCauId == LoaiYeuCau.Xoa.Value, () => // Update/Delete
        {
            RuleFor(x => x.YeuCauPhuongTienId)
                .NotEmpty()
                .WithMessage("PhuongTienId không được để trống.");
        });

        When(x => x.FileIds != null, () =>
        {
            RuleForEach(x => x.FileIds)
                .GreaterThan(0)
                .WithMessage("ID của tệp tin không hợp lệ.");
        });
    }
}
