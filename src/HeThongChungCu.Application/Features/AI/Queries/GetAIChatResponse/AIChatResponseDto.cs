using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;

public class AIChatResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public List<AIChatSourceDto> Sources { get; set; } = new();

    /// <summary>
    /// True nếu câu hỏi của cư dân đã được LLM viết lại thành Standalone Question
    /// dựa vào lịch sử hội thoại (bước Condense trong Conversational RAG).
    /// </summary>
    public bool IsCondensed { get; set; }
}


public class AIChatSourceDto
{
    public string Source { get; set; } = string.Empty;
    public string H1 { get; set; } = string.Empty;
    public string H2 { get; set; } = string.Empty;
    public string H3 { get; set; } = string.Empty;
    public float Score { get; set; }
}
