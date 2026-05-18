using HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface ILLMService
{
    /// <summary>
    /// Sinh câu trả lời từ LLM dựa trên câu hỏi, ngữ cảnh RAG và chỉ thị hệ thống.
    /// </summary>
    /// <param name="prompt">Câu hỏi của người dùng.</param>
    /// <param name="context">Ngữ cảnh bổ sung (tài liệu RAG).</param>
    /// <param name="systemInstruction">Chỉ thị hệ thống thiết lập hành vi/luật cho AI.</param>
    /// <param name="cancellationToken">Token thông báo hủy tác vụ liên kết.</param>
    /// <returns>Câu trả lời đã được sinh từ AI.</returns>
    Task<string> GenerateResponseAsync(
        string prompt,
        string context = "",
        string systemInstruction = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dùng LLM viết lại câu hỏi hiện tại thành câu hỏi độc lập, đầy đủ ngữ cảnh từ lịch sử hội thoại.
    /// Được dùng trong bước Condense của pipeline Conversational RAG trước khi tìm kiếm vector.
    /// Nếu lịch sử rỗng, trả về câu hỏi gốc không thay đổi.
    /// </summary>
    /// <param name="currentQuestion">Câu hỏi hiện tại của người dùng.</param>
    /// <param name="history">Lịch sử hội thoại gần nhất (tối đa 5 lượt).</param>
    /// <param name="cancellationToken">Token thông báo hủy tác vụ liên kết.</param>
    /// <returns>Câu hỏi độc lập đã được viết lại.</returns>
    Task<string> CondenseQuestionAsync(
        string currentQuestion,
        IReadOnlyList<ChatMessageDto> history,
        CancellationToken cancellationToken = default);
}

