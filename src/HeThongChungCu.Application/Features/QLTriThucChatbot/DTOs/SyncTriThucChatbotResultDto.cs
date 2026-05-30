namespace HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

/// <summary>Kết quả sau khi chạy đồng bộ tri thức chatbot từ SQL DB lên Qdrant.</summary>
public class SyncTriThucChatbotResultDto
{
    /// <summary>Số bản ghi đã upsert lên Qdrant (IsActive = true).</summary>
    public int UpsertedCount { get; set; }

    /// <summary>Số bản ghi đã xóa khỏi Qdrant (IsActive = false).</summary>
    public int DeletedCount { get; set; }

    /// <summary>Số bản ghi bỏ qua (đã sync rồi và SyncAll = false).</summary>
    public int SkippedCount { get; set; }

    /// <summary>Danh sách ID gặp lỗi trong quá trình sync.</summary>
    public List<int> ErrorIds { get; set; } = [];

    /// <summary>Thời gian thực hiện (ms).</summary>
    public long ElapsedMs { get; set; }
}
