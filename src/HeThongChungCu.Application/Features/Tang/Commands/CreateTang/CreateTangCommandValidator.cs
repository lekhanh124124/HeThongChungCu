using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandValidator : AbstractValidator<CreateTangCommand>
{
    public CreateTangCommandValidator()
    {
        RuleFor(x => x.MaTang)
            .NotEmpty().WithMessage("Mã tầng không được để trống.")
            .MaximumLength(20).WithMessage("Mã tầng không được vượt quá 20 ký tự.");

        RuleFor(x => x.TenTang)
            .NotEmpty().WithMessage("Tên tầng không được để trống.")
            .MaximumLength(100).WithMessage("Tên tầng không được vượt quá 100 ký tự.");

        RuleFor(x => x.ToaNhaId)
            .GreaterThan(0).WithMessage("ID tòa nhà không hợp lệ.");

        RuleFor(x => x.LoaiTangId)
            .Must(id => LoaiTang.GetAll().Any(g => g.Value == id))
            .WithMessage($"Loại tầng không hợp lệ. Các giá trị hợp lệ: " +
                         $"{string.Join(", ", LoaiTang.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
