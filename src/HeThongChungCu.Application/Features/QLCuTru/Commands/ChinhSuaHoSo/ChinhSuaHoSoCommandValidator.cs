using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;

public class ChinhSuaHoSoCommandValidator : AbstractValidator<ChinhSuaHoSoCommand>
{
    public ChinhSuaHoSoCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0).WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống.")
            .MaximumLength(100).WithMessage("Họ không được vượt quá 100 ký tự.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(100).WithMessage("Tên không được vượt quá 100 ký tự.");
        RuleFor(x => x.Dob).NotEmpty().WithMessage("Ngày sinh không được để trống.");
        RuleFor(x => x.GioiTinhId).InclusiveBetween(1, 2).WithMessage("Giá trị Giới tính phải nằm trong khoảng từ 1 đến 2.");
        RuleFor(x => x.DiaChi).MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự.");
        RuleFor(x => x.LoaiQuanHeCuTruId).GreaterThan(0).WithMessage("Giá trị Quan hệ cư trú phải nằm trong khoảng từ 1 đến 2147483647.");
    }
}
