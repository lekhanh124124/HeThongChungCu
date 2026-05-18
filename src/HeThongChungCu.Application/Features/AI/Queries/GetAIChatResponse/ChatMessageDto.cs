namespace HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;

/// <summary>
/// Đại diện cho một lượt hội thoại trong lịch sử chat (người dùng hoặc trợ lý).
/// </summary>
public class ChatMessageDto
{
    /// <summary>Vai trò của người gửi: "user" (cư dân) hoặc "assistant" (trợ lý ảo).</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Nội dung tin nhắn.</summary>
    public string Content { get; set; } = string.Empty;
}
