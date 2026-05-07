using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;

public class GetLichBaoTriListQueryHandler : IQueryHandler<GetLichBaoTriListQuery, PagedResult<LichBaoTriResponse>>
{
    private readonly ILichBaoTriQueryRepository _queryRepository;

    public GetLichBaoTriListQueryHandler(ILichBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<LichBaoTriResponse>>> Handle(GetLichBaoTriListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetLichBaoTriListSpecification(
            request.ThietBiId,
            request.HangMucId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
