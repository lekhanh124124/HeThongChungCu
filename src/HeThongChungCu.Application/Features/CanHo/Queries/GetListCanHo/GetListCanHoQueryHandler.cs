using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

public class GetListCanHoQueryHandler : IQueryHandler<GetListCanHoQuery, PagedResult<CanHoDetailResponse>>
{
    private readonly ICanHoQueryRepository _queryRepository;

    public GetListCanHoQueryHandler(ICanHoQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<CanHoDetailResponse>>> Handle(GetListCanHoQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListCanHoSpecification(
            request.TangId,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);

        return Result.Success(result);
    }
}
