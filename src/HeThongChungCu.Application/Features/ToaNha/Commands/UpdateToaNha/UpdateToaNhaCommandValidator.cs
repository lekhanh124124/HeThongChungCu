using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Giá trị Tòa nhà phải nằm trong khoảng từ 1 đến 2147483647.");

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage("Tên tòa nhà không được để trống.")
            .MaximumLength(100).WithMessage("Tên tòa nhà không được vượt quá 100 ký tự.");

        RuleFor(x => x.Block)
            .NotEmpty().WithMessage("Block không được để trống.")
            .Length(1).WithMessage("Block phải là 1 ký tự.")
            .Must(b => char.IsLetter(b[0]) && char.IsUpper(b[0])).WithMessage("Block phải là một ký tự alphabet in hoa (A-Z).");


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Địa chỉ không được để trống.")
            .MaximumLength(255).WithMessage("Địa chỉ không được vượt quá 255 ký tự.");

        RuleFor(x => x.TrangThaiToaNhaId)
            .Must(id => TrangThaiToaNha.GetAll().Any(g => g.Value == id))
            .WithMessage($"Trạng thái tòa nhà không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", TrangThaiToaNha.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
