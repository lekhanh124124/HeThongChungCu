using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetAllCanHos;

public class GetAllCanHosQueryHandler : IQueryHandler<GetAllCanHosQuery, PagedResult<CanHoDetailResponse>>
{
    private readonly ICanHoDapperRepository _queryRepository;

    public GetAllCanHosQueryHandler(ICanHoDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<CanHoDetailResponse>>> Handle(GetAllCanHosQuery request, CancellationToken cancellationToken)
    {
        var (totalCount, items) = await _queryRepository.GetAllAsync(
            request.ToaNhaId,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(new PagedResult<CanHoDetailResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount
            }
        });
    }
}
