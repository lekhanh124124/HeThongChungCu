using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public class CreateKhungGioDichVuCommandValidator : AbstractValidator<CreateKhungGioDichVuCommand>
{
    public CreateKhungGioDichVuCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage("Giá trị Dịch vụ phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TenKhungGio)
            .NotEmpty().WithMessage("Tên khung giờ không được để trống.")
            .MaximumLength(100).WithMessage("Tên khung giờ không được vượt quá 100 ký tự.");

        RuleFor(x => x.GioBatDau)
            .NotEmpty().WithMessage("Giờ bắt đầu không được để trống.");

        RuleFor(x => x.GioKetThuc)
            .NotEmpty().WithMessage("Giờ kết thúc không được để trống.")
            .GreaterThan(x => x.GioBatDau).WithMessage("Giờ kết thúc phải lớn hơn giờ bắt đầu.");

        RuleFor(x => x.NgayTrongTuan)
            .InclusiveBetween(0, 6).When(x => x.NgayTrongTuan.HasValue)
            .WithMessage("Ngày trong tuần phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
    }
}
