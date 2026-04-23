using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.UpdateYeuCauThiCong;

public class UpdateYeuCauThiCongCommandValidator : AbstractValidator<UpdateYeuCauThiCongCommand>
{
    public UpdateYeuCauThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.HangMucThiCong).MaximumLength(500).WithMessage("Hạng mục thi công phải có độ dài từ 1 đến 500 ký tự.");
        RuleFor(x => x.TenDonViThiCong).MaximumLength(200).WithMessage("Tên đơn vị thi công phải có độ dài từ 1 đến 200 ký tự.");
        RuleFor(x => x.NguoiDaiDien).MaximumLength(100).WithMessage("Người đại diện phải có độ dài từ 1 đến 100 ký tự.");

        RuleFor(x => x.DuKienKetThuc)
            .GreaterThan(x => x.DuKienBatDau)
            .When(x => x.DuKienBatDau.HasValue && x.DuKienKetThuc.HasValue)
            .WithMessage("Ngày kết thúc phải sau ngày bắt đầu.");

        RuleForEach(x => x.DanhSachNhanSu).ChildRules(nhanSu =>
        {
            nhanSu.RuleFor(n => n.HoTen).NotEmpty().WithMessage("Họ tên nhân sự không được để trống.");
            nhanSu.RuleFor(n => n.SoCCCD).NotEmpty().WithMessage("Số CCCD không được để trống.");
        });
    }
}
