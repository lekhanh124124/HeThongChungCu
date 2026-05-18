using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Commands.SyncKnowledgeBase;

public class SyncKnowledgeBaseCommandValidator : AbstractValidator<SyncKnowledgeBaseCommand>
{
    public SyncKnowledgeBaseCommandValidator()
    {
        RuleFor(x => x.MaxFilesToSync)
            .GreaterThan(0)
                .WithMessage("Số lượng tệp tối đa phải lớn hơn 0.")
            .LessThanOrEqualTo(500)
                .WithMessage("Số lượng tệp tối đa không được vượt quá 500.")
            .When(x => x.MaxFilesToSync.HasValue);
    }
}
