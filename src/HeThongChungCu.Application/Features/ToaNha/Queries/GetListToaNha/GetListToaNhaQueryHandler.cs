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
        var spec = new GetListToaNhaSpecification(
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);

        return Result.Success(result);
    }
}

