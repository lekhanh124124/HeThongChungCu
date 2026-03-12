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
        var spec = new GetListTangSpecification(
            request.ToaNhaId,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);
        
        return Result.Success(result);
    }
}
