using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Commands.TestBatchSearch;

public class TestBatchSearchCommandValidator : AbstractValidator<TestBatchSearchCommand>
{
    public TestBatchSearchCommandValidator()
    {
        RuleFor(x => x.CollectionName)
            .NotEmpty()
                .WithMessage("Tên collection không được để trống.")
            .MaximumLength(200)
                .WithMessage("Tên collection không được vượt quá 200 ký tự.");

        RuleFor(x => x.Texts)
            .NotNull()
                .WithMessage("Danh sách Texts không được null.")
            .NotEmpty()
                .WithMessage("Danh sách Texts không được để trống.")
            .Must(texts => texts.Count <= 50)
                .WithMessage("Danh sách Texts không được vượt quá 50 mục.");

        RuleForEach(x => x.Texts)
            .NotEmpty()
                .WithMessage("Mỗi đoạn văn bản trong danh sách không được để trống.")
            .MaximumLength(5000)
                .WithMessage("Mỗi đoạn văn bản không được vượt quá 5000 ký tự.");
    }
}
