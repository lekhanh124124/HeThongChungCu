using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

public class GetListToaNhaQueryHandler : IQueryHandler<GetListToaNhaQuery, PagedResult<ToaNhaDetailResponse>>
{
    private readonly IToaNhaDapperRepository _queryRepository;

    public GetListToaNhaQueryHandler(IToaNhaDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<ToaNhaDetailResponse>>> Handle(GetListToaNhaQuery request, CancellationToken cancellationToken)
    {
        var (totalCount, items) = await _queryRepository.GetAllAsync(
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(new PagedResult<ToaNhaDetailResponse>
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

