using HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;

public class CreateToaNhaCommandValidator : AbstractValidator<CreateToaNhaCommand>
{
    public CreateToaNhaCommandValidator()
    {
        RuleFor(x => x.MaToaNha)
            .NotEmpty().WithMessage("MÃ£ tÃ²a nhÃ  khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.")
            .MaximumLength(20).WithMessage("MÃ£ tÃ²a nhÃ  khÃ´ng Ä‘Æ°á»£c vÆ°á»£t quÃ¡ 20 kÃ½ tá»±.");

        RuleFor(x => x.TenToaNha)
            .NotEmpty().WithMessage("TÃªn tÃ²a nhÃ  khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.")
            .MaximumLength(100).WithMessage("TÃªn tÃ²a nhÃ  khÃ´ng Ä‘Æ°á»£c vÆ°á»£t quÃ¡ 100 kÃ½ tá»±.");

        RuleFor(x => x.SoTang)
            .GreaterThan(0).WithMessage("Sá»‘ táº§ng pháº£i lá»›n hÆ¡n 0.");

        RuleFor(x => x.SoTangHam)
            .GreaterThan(0).WithMessage("Sá»‘ táº§ng háº§m pháº£i lá»›n hÆ¡n 0.");

        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Äá»‹a chá»‰ khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.")
            .MaximumLength(255).WithMessage("Äá»‹a chá»‰ khÃ´ng Ä‘Æ°á»£c vÆ°á»£t quÃ¡ 255 kÃ½ tá»±.");

    }
}
