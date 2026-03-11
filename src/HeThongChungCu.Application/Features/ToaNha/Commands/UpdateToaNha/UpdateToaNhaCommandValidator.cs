using HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID tòa nhà không hợp lệ.");

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage("Tên tòa nhà không được để trống.")
            .MaximumLength(100).WithMessage("Tên tòa nhà không được vượt quá 100 ký tự.");


        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Địa chỉ toàn nhà không được để trống.")
            .MaximumLength(255).WithMessage("Địa chỉ toàn nhà không được vượt quá 255 ký tự.");

        RuleFor(x => x.TrangThaiToaNhaId)
            .Must(id => TrangThaiToaNha.GetAll().Any(g => g.Value == id))
            .WithMessage($"Trạng thái tòa nhà không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", TrangThaiToaNha.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
