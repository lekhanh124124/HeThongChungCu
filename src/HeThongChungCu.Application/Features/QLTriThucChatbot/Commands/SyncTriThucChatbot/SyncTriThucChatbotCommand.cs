using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.SyncTriThucChatbot;

/// <summary>
/// Đồng bộ tri thức chatbot từ SQL DB lên Qdrant vector store.
/// Sau mỗi lần gọi, Qdrant sẽ phản ánh đúng trạng thái hiện tại của DB:
/// - IsActive = true  → upsert vector (idempotent).
/// - IsActive = false (đã từng sync) → xóa vector khỏi Qdrant.
/// </summary>
public record SyncTriThucChatbotCommand : ICommand<SyncTriThucChatbotResultDto>;
