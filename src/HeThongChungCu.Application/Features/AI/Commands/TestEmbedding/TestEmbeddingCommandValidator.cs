using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Commands.TestEmbedding;

public class TestEmbeddingCommandValidator : AbstractValidator<TestEmbeddingCommand>
{
    public TestEmbeddingCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
                .WithMessage("Text không được để trống.")
            .MaximumLength(5000)
                .WithMessage("Text không được vượt quá 5000 ký tự.");
    }
}
