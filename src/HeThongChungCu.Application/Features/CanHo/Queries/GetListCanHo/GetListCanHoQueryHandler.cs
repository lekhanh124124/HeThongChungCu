using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

public class GetListCanHoQueryHandler : IQueryHandler<GetListCanHoQuery, PagedResult<CanHoDetailResponse>>
{
    private readonly ICanHoDapperRepository _queryRepository;

    public GetListCanHoQueryHandler(ICanHoDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<CanHoDetailResponse>>> Handle(GetListCanHoQuery request, CancellationToken cancellationToken)
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
