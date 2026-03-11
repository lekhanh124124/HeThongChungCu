using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;

public class LayCauTrucChungCuQueryHandler : IQueryHandler<LayCauTrucChungCuQuery, IReadOnlyList<CauTrucToaNhaResponse>>
{
    private readonly IToaNhaDapperRepository _toaNhaDapperRepository;

    public LayCauTrucChungCuQueryHandler(IToaNhaDapperRepository toaNhaDapperRepository)
    {
        _toaNhaDapperRepository = toaNhaDapperRepository;
    }

    public async Task<Result<IReadOnlyList<CauTrucToaNhaResponse>>> Handle(LayCauTrucChungCuQuery request, CancellationToken cancellationToken)
    {
        var result = await _toaNhaDapperRepository.GetCauTrucChungCuAsync(request.Keyword, cancellationToken);
        return Result.Success(result);
    }

}
