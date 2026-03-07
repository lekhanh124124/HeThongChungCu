using HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID tòa nhà không hợp lệ.");

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage("Tên tòa nhà không được để trống.")
            .MaximumLength(100).WithMessage("Tên tòa nhà không được vượt quá 100 ký tự.");

        RuleFor(x => x.SoTang)
            .GreaterThan(0).WithMessage("Số tầng phải lớn hơn 0.");
    }
}
