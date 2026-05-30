namespace HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

/// <summary>Kết quả sau khi import file Markdown vào kho tri thức chatbot.</summary>
public class ImportTriThucChatbotResultDto
{
    /// <summary>Số mục tri thức đã được tạo thành công.</summary>
    public int ImportedCount { get; set; }

    /// <summary>DanhMuc được trích xuất từ H1 của file.</summary>
    public string DanhMuc { get; set; } = string.Empty;

    /// <summary>Danh sách tiêu đề (TieuDe) của các mục đã tạo.</summary>
    public List<string> ImportedTitles { get; set; } = [];
}
