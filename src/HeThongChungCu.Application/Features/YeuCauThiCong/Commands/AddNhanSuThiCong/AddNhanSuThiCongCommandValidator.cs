using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddNhanSuThiCong;

public class AddNhanSuThiCongCommandValidator : AbstractValidator<AddNhanSuThiCongCommand>
{
    public AddNhanSuThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.HoTen).NotEmpty().MaximumLength(100).WithMessage("Họ tên không được để trống và tối đa 100 ký tự.");
        RuleFor(x => x.SoCCCD).NotEmpty().MaximumLength(20).WithMessage("Số CCCD không được để trống và tối đa 20 ký tự.");
        RuleFor(x => x.SoDienThoai).MaximumLength(20).WithMessage("Số điện thoại tối đa 20 ký tự.");
        RuleFor(x => x.VaiTro).MaximumLength(50).WithMessage("Vai trò tối đa 50 ký tự.");
    }
}
