using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriList;

public class GetPhieuBaoTriListQueryHandler : IQueryHandler<GetPhieuBaoTriListQuery, PagedResult<PhieuBaoTriResponse>>
{
    private readonly IPhieuBaoTriQueryRepository _queryRepository;

    public GetPhieuBaoTriListQueryHandler(IPhieuBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<PhieuBaoTriResponse>>> Handle(GetPhieuBaoTriListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetPhieuBaoTriListSpecification(
            request.Keyword,
            request.TrangThaiPhieuBaoTriId,
            request.ThietBiId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
