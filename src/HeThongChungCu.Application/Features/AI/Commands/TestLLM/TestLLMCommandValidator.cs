using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Commands.TestLLM;

public class TestLLMCommandValidator : AbstractValidator<TestLLMCommand>
{
    public TestLLMCommandValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty()
                .WithMessage("Prompt không được để trống.")
            .MaximumLength(5000)
                .WithMessage("Prompt không được vượt quá 5000 ký tự.");
    }
}
