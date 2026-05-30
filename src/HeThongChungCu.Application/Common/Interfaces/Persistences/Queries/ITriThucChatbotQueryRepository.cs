using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface ITriThucChatbotQueryRepository
{
    Task<TriThucChatbotResponse?> GetByIdAsync(
        GetTriThucChatbotByIdSpecification spec,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TriThucChatbotResponse>> GetListAsync(
        GetListTriThucChatbotSpecification spec,
        CancellationToken cancellationToken = default);
}
