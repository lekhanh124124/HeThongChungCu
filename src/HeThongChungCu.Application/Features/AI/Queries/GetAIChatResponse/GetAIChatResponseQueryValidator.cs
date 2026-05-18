using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;

public class GetAIChatResponseQueryValidator : AbstractValidator<GetAIChatResponseQuery>
{
    private static readonly string[] ValidRoles = ["user", "assistant"];

    public GetAIChatResponseQueryValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty()
                .WithMessage("Câu hỏi không được để trống.")
            .MinimumLength(2)
                .WithMessage("Câu hỏi phải có ít nhất 2 ký tự.")
            .MaximumLength(2000)
                .WithMessage("Câu hỏi không được vượt quá 2000 ký tự.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20)
                .WithMessage("Số lượng kết quả phải nằm trong khoảng từ 1 đến 20.");

        RuleFor(x => x.DocumentType)
            .MaximumLength(200)
                .WithMessage("Tên loại tài liệu không được vượt quá 200 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentType));

        RuleFor(x => x.History)
            .Must(h => h.Count <= 50)
                .WithMessage("Lịch sử hội thoại không được vượt quá 50 lượt.")
            .When(x => x.History != null);

        RuleForEach(x => x.History)
            .ChildRules(msg =>
            {
                msg.RuleFor(m => m.Role)
                    .NotEmpty()
                        .WithMessage("Vai trò (Role) trong lịch sử không được để trống.")
                    .Must(r => ValidRoles.Contains(r?.ToLowerInvariant()))
                        .WithMessage("Vai trò (Role) phải là 'user' hoặc 'assistant'.");

                msg.RuleFor(m => m.Content)
                    .NotEmpty()
                        .WithMessage("Nội dung tin nhắn trong lịch sử không được để trống.")
                    .MaximumLength(2000)
                        .WithMessage("Nội dung tin nhắn trong lịch sử không được vượt quá 2000 ký tự.");
            })
            .When(x => x.History != null && x.History.Count > 0);
    }
}
