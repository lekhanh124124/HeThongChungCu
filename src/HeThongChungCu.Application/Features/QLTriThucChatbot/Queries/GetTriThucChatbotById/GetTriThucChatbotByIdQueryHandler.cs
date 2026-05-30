using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;

public class GetTriThucChatbotByIdQueryHandler : IQueryHandler<GetTriThucChatbotByIdQuery, TriThucChatbotResponse>
{
    private readonly ITriThucChatbotQueryRepository _queryRepository;

    public GetTriThucChatbotByIdQueryHandler(ITriThucChatbotQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<TriThucChatbotResponse>> Handle(
        GetTriThucChatbotByIdQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetTriThucChatbotByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return result is not null
            ? Result.Success(result)
            : Result.Failure<TriThucChatbotResponse>(TriThucChatbotErrors.NotFound);
    }
}
