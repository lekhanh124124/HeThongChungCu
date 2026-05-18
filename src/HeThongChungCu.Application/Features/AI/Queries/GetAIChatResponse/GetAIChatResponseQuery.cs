using HeThongChungCu.Application.Common.Messaging;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;

public class GetAIChatResponseQuery : IQuery<AIChatResponseDto>
{
    public string Prompt { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public int Limit { get; set; } = 5;

    /// <summary>
    /// Lịch sử hội thoại của phiên chat hiện tại (tối đa 50 lượt).
    /// Handler sẽ tự áp dụng sliding window 5 lượt cuối.
    /// Để rỗng cho Single-turn Q&amp;A.
    /// </summary>
    public List<ChatMessageDto> History { get; set; } = new();

    public GetAIChatResponseQuery()
    {
    }

    public GetAIChatResponseQuery(string prompt, string? documentType = null, int limit = 5, List<ChatMessageDto>? history = null)
    {
        Prompt = prompt;
        DocumentType = documentType;
        Limit = limit;
        History = history ?? new();
    }
}
