using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CreateYeuCauThiCong;

public class CreateYeuCauThiCongCommandValidator : AbstractValidator<CreateYeuCauThiCongCommand>
{
    public CreateYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.CanHoId).NotEmpty().WithMessage("CanHoId không được để trống.");
        RuleFor(x => x.HangMucThiCong).NotEmpty().MaximumLength(500).WithMessage("HangMucThiCong không được để trống.");
        RuleFor(x => x.DuKienBatDau).NotEmpty().WithMessage("DuKienBatDau không được để trống.");
        RuleFor(x => x.DuKienKetThuc).NotEmpty().GreaterThan(x => x.DuKienBatDau).WithMessage("DuKienKetThuc phải lớn hơn DuKienBatDau.");
        RuleFor(x => x.TenDonViThiCong).MaximumLength(200).WithMessage("TenDonViThiCong không được vượt quá 200 ký tự.");
        RuleFor(x => x.NguoiDaiDien).MaximumLength(100).WithMessage("NguoiDaiDien không được vượt quá 100 ký tự.");
        RuleFor(x => x.SoDienThoaiDaiDien).MaximumLength(100).WithMessage("SoDienThoaiDaiDien không được vượt quá 100 ký tự.");
        RuleFor(x => x.DanhSachTepIds).NotNull().WithMessage("DanhSachTepIds không được để trống.");
        RuleFor(x => x.IsSubmit).Must(x => x || !x).WithMessage("IsSubmit không được để trống.");
        
        RuleForEach(x => x.DanhSachNhanSu).ChildRules(nhanSu => {
            nhanSu.RuleFor(n => n.HoTen).NotEmpty().WithMessage("Họ tên nhân sự không được để trống.");
            nhanSu.RuleFor(n => n.SoCCCD).NotEmpty().WithMessage("Số CCCD không được để trống.");
        });
    }
}
