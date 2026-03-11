using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetListTang;

public class GetListTangQueryHandler : IQueryHandler<GetListTangQuery, PagedResult<TangDetailResponse>>
{
    private readonly ITangDapperRepository _queryRepository;

    public GetListTangQueryHandler(ITangDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<TangDetailResponse>>> Handle(GetListTangQuery request, CancellationToken cancellationToken)
    {
        var (totalCount, items) = await _queryRepository.GetAllAsync(
            request.ToaNhaId,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(new PagedResult<TangDetailResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 20,
                TotalItems = totalCount
            }
        });
    }
}
