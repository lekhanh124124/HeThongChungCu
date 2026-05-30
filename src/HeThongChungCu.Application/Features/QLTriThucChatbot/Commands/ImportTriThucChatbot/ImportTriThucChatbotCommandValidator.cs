using FluentValidation;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ImportTriThucChatbot;

public class ImportTriThucChatbotCommandValidator : AbstractValidator<ImportTriThucChatbotCommand>
{
    private static readonly HashSet<string> AllowedExtensions = [".md"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public ImportTriThucChatbotCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Tên file không được để trống.")
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage("Chỉ hỗ trợ định dạng Markdown (.md).");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File không được rỗng.")
            .Must(s => s.Length > 0).WithMessage("File không có nội dung.")
            .Must(s => s.Length <= MaxFileSizeBytes)
            .WithMessage($"File không được vượt quá {MaxFileSizeBytes / 1024 / 1024} MB.");
    }
}
