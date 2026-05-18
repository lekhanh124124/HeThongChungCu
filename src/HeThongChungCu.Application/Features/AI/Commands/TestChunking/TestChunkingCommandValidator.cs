using FluentValidation;

namespace HeThongChungCu.Application.Features.AI.Commands.TestChunking;

public class TestChunkingCommandValidator : AbstractValidator<TestChunkingCommand>
{
    private static readonly string[] AllowedExtensions = [".md"];

    public TestChunkingCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
                .WithMessage("Tên file không được để trống.")
            .Must(HaveAllowedExtension)
                .WithMessage("Hệ thống hiện tại chỉ hỗ trợ kiểm tra chunking với định dạng file .md");

        RuleFor(x => x.ChunkSize)
            .GreaterThan(0)
                .WithMessage("Kích thước chunk phải lớn hơn 0.")
            .LessThanOrEqualTo(2000)
                .WithMessage("Kích thước chunk không được vượt quá 2000 token.");

        RuleFor(x => x.ChunkOverlap)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Độ chồng lấp không được âm.")
            .LessThan(x => x.ChunkSize)
                .WithMessage("Độ chồng lấp phải nhỏ hơn kích thước chunk.");
    }

    private static bool HaveAllowedExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var extension = System.IO.Path.GetExtension(fileName);
        return AllowedExtensions.Contains(extension.ToLowerInvariant());
    }
}
