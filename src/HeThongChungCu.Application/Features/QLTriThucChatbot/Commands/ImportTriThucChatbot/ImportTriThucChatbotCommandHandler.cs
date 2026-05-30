using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ImportTriThucChatbot;

public class ImportTriThucChatbotCommandHandler
    : ICommandHandler<ImportTriThucChatbotCommand, ImportTriThucChatbotResultDto>
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImportTriThucChatbotCommandHandler> _logger;

    public ImportTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ImportTriThucChatbotCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<Result<ImportTriThucChatbotResultDto>> Handle(
        ImportTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate extension
        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (ext != ".md")
            return Result.Failure<ImportTriThucChatbotResultDto>(TriThucChatbotErrors.InvalidFileFormat);

        // 2. Đọc nội dung file
        using var reader = new StreamReader(request.FileStream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<ImportTriThucChatbotResultDto>(TriThucChatbotErrors.EmptyFile);

        // 3. Parse: trích xuất DanhMuc (H1) và TieuDe (H1 hoặc tên file)
        //    Toàn bộ nội dung còn lại sau H1 là NoiDung — 1 file = 1 bản ghi.
        var (tieuDe, danhMuc, noiDung) = ParseMarkdown(content, request.FileName);

        if (string.IsNullOrWhiteSpace(noiDung))
            return Result.Failure<ImportTriThucChatbotResultDto>(TriThucChatbotErrors.EmptyFile);

        // 4. Tạo 1 bản ghi duy nhất
        //    Nếu admin truyền DanhMucOverride thì ưu tiên dùng, ngược lại dùng TieuDe từ H1.
        var finalDanhMuc = !string.IsNullOrWhiteSpace(request.DanhMucOverride)
            ? request.DanhMucOverride.Trim()
            : danhMuc;

        var createResult = TriThucChatbot.CreateTriThucChatbot(
            tieuDe,
            noiDung,
            finalDanhMuc,
            request.ThuTuHienThi);

        if (createResult.IsFailure)
            return Result.Failure<ImportTriThucChatbotResultDto>(createResult.Errors[0]);

        _repository.Add(createResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ImportTriThucChatbot: Đã tạo 1 mục tri thức từ file '{FileName}'. TieuDe='{TieuDe}', DanhMuc='{DanhMuc}'.",
            request.FileName, tieuDe, finalDanhMuc);

        return Result.Success(new ImportTriThucChatbotResultDto
        {
            ImportedCount  = 1,
            DanhMuc        = finalDanhMuc,
            ImportedTitles = [tieuDe]
        });
    }

    // ─── Markdown Parser ────────────────────────────────────────────────────

    /// <summary>
    /// Parse file Markdown:
    /// - H1 (#) đầu tiên → TieuDe và DanhMuc.
    /// - Nội dung toàn bộ sau H1 → NoiDung (giữ nguyên Markdown, kể cả các H2, H3...).
    /// - Nếu không có H1, dùng tên file (không đuôi) làm TieuDe, "chung" làm DanhMuc.
    /// </summary>
    private static (string TieuDe, string DanhMuc, string NoiDung) ParseMarkdown(
        string content, string fileName)
    {
        var lines = content.Split('\n').Select(l => l.TrimEnd()).ToList();

        string? tieuDe = null;
        int contentStartIndex = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            // H1 thực sự: bắt đầu bằng "# " nhưng không phải "## " trở lên
            if (line.StartsWith("# ") && !line.StartsWith("## "))
            {
                tieuDe = line[2..].Trim();
                contentStartIndex = i + 1;
                break;
            }
        }

        if (string.IsNullOrEmpty(tieuDe))
        {
            // Không có H1 → dùng tên file làm tiêu đề
            tieuDe = Path.GetFileNameWithoutExtension(fileName).Trim();
            contentStartIndex = 0;
        }

        var noiDung = string.Join('\n', lines.Skip(contentStartIndex)).Trim();

        return (tieuDe, tieuDe, noiDung); // DanhMuc = TieuDe (từ H1)
    }
}
