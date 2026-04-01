using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

public class GetListToaNhaQueryHandler : IQueryHandler<GetListToaNhaQuery, PagedResult<ToaNhaDetailResponse>>
{
    private readonly IToaNhaQueryRepository _queryRepository;

    public GetListToaNhaQueryHandler(IToaNhaQueryRepository queryRepository)
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

