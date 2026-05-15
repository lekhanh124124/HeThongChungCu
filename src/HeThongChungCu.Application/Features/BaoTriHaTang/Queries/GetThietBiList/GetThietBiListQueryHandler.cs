using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;

public class GetThietBiListQueryHandler : IQueryHandler<GetThietBiListQuery, PagedResult<ThietBiResponse>>
{
    private readonly IThietBiQueryRepository _thietBiQueryRepository;

    public GetThietBiListQueryHandler(IThietBiQueryRepository thietBiQueryRepository)
    {
        _thietBiQueryRepository = thietBiQueryRepository;
    }

    public async Task<Result<PagedResult<ThietBiResponse>>> Handle(GetThietBiListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetThietBiListSpecification(
            request.Keyword,
            request.TrangThaiThietBiId,
            request.ToaNhaId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _thietBiQueryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
