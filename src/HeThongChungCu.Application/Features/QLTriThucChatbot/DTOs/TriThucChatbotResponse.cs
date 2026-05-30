namespace HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

/// <summary>Kết quả trả về sau khi tạo / cập nhật / truy vấn một mục tri thức chatbot.</summary>
public class TriThucChatbotResponse
{
    public int Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string DanhMuc { get; set; } = string.Empty;
    public int ThuTuHienThi { get; set; }
    public bool IsActive { get; set; }
    public bool IsSynced { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
