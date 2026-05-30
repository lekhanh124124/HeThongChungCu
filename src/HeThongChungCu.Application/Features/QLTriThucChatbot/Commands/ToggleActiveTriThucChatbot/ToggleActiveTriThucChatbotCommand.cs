using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ToggleActiveTriThucChatbot;

/// <summary>Bật hoặc tắt một mục tri thức. Khi tắt sẽ không sync lên Qdrant.</summary>
public record ToggleActiveTriThucChatbotCommand(int Id, bool Activate) : ICommand<bool>;
