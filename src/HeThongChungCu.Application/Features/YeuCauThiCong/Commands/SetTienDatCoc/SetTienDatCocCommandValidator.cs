using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.SetTienDatCoc;

public class SetTienDatCocCommandValidator : AbstractValidator<SetTienDatCocCommand>
{
    public SetTienDatCocCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.SoTien).GreaterThanOrEqualTo(0).WithMessage("Số tiền phải lớn hơn hoặc bằng 0.");
    }
}
