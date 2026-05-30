using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ImportTriThucChatbot;

/// <summary>
/// Import một file Markdown vào kho tri thức chatbot dưới dạng 1 bản ghi duy nhất.
/// - TieuDe: lấy từ H1 (#) đầu tiên trong file, hoặc tên file nếu không có H1.
/// - DanhMuc: mặc định bằng TieuDe (từ H1), có thể ghi đè bằng <see cref="DanhMucOverride"/>.
/// - NoiDung: toàn bộ nội dung còn lại sau H1 (giữ nguyên Markdown).
/// Sau import cần gọi /sync để đồng bộ lên Qdrant.
/// </summary>
public record ImportTriThucChatbotCommand(
    Stream FileStream,
    string FileName,
    int ThuTuHienThi = 0,
    string? DanhMucOverride = null) : ICommand<ImportTriThucChatbotResultDto>;
