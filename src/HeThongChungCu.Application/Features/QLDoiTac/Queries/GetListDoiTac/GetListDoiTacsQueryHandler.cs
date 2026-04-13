using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;

public class GetListDoiTacsQueryHandler : IQueryHandler<GetListDoiTacsQuery, PagedResult<DoiTacResponse>>
{
    private readonly IDoiTacQueryRepository _doiTacQueryRepository;

    public GetListDoiTacsQueryHandler(IDoiTacQueryRepository doiTacQueryRepository)
    {
        _doiTacQueryRepository = doiTacQueryRepository;
    }

    public async Task<Result<PagedResult<DoiTacResponse>>> Handle(GetListDoiTacsQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListDoiTacSpecification(
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);
        var result = await _doiTacQueryRepository.GetAllAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
