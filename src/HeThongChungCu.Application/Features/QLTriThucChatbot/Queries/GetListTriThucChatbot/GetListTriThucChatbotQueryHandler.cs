using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;

public class GetListTriThucChatbotQueryHandler
    : IQueryHandler<GetListTriThucChatbotQuery, PagedResult<TriThucChatbotResponse>>
{
    private readonly ITriThucChatbotQueryRepository _queryRepository;

    public GetListTriThucChatbotQueryHandler(ITriThucChatbotQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<TriThucChatbotResponse>>> Handle(
        GetListTriThucChatbotQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetListTriThucChatbotSpecification(
            request.DanhMuc,
            request.IsActive,
            request.IsSynced,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
