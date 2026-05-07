using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;

public class GetHangMucBaoTriListQueryHandler : IQueryHandler<GetHangMucBaoTriListQuery, PagedResult<HangMucBaoTriResponse>>
{
    private readonly IHangMucBaoTriQueryRepository _queryRepository;

    public GetHangMucBaoTriListQueryHandler(IHangMucBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<HangMucBaoTriResponse>>> Handle(GetHangMucBaoTriListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetHangMucBaoTriListSpecification(
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
