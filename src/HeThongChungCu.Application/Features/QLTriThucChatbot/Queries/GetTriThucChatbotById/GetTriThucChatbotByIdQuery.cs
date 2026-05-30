using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;

public record GetTriThucChatbotByIdQuery(int Id) : IQuery<TriThucChatbotResponse>;
