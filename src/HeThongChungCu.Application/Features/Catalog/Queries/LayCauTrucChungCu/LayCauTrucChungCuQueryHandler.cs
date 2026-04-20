using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;

public class LayCauTrucChungCuQueryHandler : IQueryHandler<LayCauTrucChungCuQuery, List<CauTrucToaNhaResponse>>
{
    private readonly IToaNhaQueryRepository _toaNhaQueryRepository;

    public LayCauTrucChungCuQueryHandler(IToaNhaQueryRepository toaNhaQueryRepository)
    {
        _toaNhaQueryRepository = toaNhaQueryRepository;
    }

    public async Task<Result<List<CauTrucToaNhaResponse>>> Handle(LayCauTrucChungCuQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayCauTrucChungCuSpecification(request.Keyword);
        var result = await _toaNhaQueryRepository.GetCauTrucChungCuAsync(spec, cancellationToken);
        return result.ToList();
    }

}
