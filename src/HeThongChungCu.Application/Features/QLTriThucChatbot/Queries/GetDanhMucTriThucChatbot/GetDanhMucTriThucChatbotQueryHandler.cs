using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetDanhMucTriThucChatbot;

public class GetDanhMucTriThucChatbotQueryHandler
    : IQueryHandler<GetDanhMucTriThucChatbotQuery, List<string>>
{
    private readonly ITriThucChatbotQueryRepository _queryRepository;

    public GetDanhMucTriThucChatbotQueryHandler(ITriThucChatbotQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<List<string>>> Handle(
        GetDanhMucTriThucChatbotQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _queryRepository.GetDanhMucListAsync(cancellationToken);
        return Result.Success(result);
    }
}
