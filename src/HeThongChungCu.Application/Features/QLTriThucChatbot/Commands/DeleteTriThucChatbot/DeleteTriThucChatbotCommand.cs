using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.DeleteTriThucChatbot;

public record DeleteTriThucChatbotCommand(List<int> Ids) : ICommand<bool>;
