namespace HeThongChungCu.Application.Common.Interfaces.Services;

/// <summary>
/// Dịch vụ bổ sung ngữ cảnh thời gian thực vào pipeline chatbot.
/// Phát hiện intent từ câu hỏi của người dùng và truy vấn dữ liệu động từ DB
/// (DichVu, BangGia, ThongBao...) để inject vào prompt trước khi gọi LLM.
/// </summary>
public interface IChatbotContextEnricher
{
    /// <summary>
    /// Phân tích câu hỏi, truy vấn DB và trả về chuỗi ngữ cảnh bổ sung.
    /// Trả về <see cref="string.Empty"/> nếu không có dữ liệu động liên quan.
    /// </summary>
    /// <param name="prompt">Câu hỏi gốc của người dùng (đã hoặc chưa condense).</param>
    /// <param name="cancellationToken">Token hủy.</param>
    Task<string> EnrichAsync(string prompt, CancellationToken cancellationToken = default);
}
