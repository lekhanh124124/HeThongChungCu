using HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;

public class UpdateToaNhaCommandValidator : AbstractValidator<UpdateToaNhaCommand>
{
    public UpdateToaNhaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID tÃ²a nhÃ  khÃ´ng há»£p lá»‡.");

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

        RuleFor(x => x.TrangThaiToaNhaId)
            .Must(id => TrangThaiToaNha.GetAll().Any(g => g.Value == id))
            .WithMessage($"Tráº¡ng thÃ¡i tÃ²a nhÃ  khÃ´ng há»£p lá»‡. CÃ¡c giÃ¡ trá»‹ há»£p lá»‡: " +
                         $"{string.Join(", ", TrangThaiToaNha.GetAll().Select(l => $"{l.Value} ({l.Name})"))}.");
    }
}
