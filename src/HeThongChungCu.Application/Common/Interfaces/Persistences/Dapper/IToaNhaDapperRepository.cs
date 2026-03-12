using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IToaNhaDapperRepository
{
    Task<PagedResult<ToaNhaDetailResponse>> GetAllAsync(
        GetListToaNhaSpecification spec,
        CancellationToken cancellationToken = default);

    Task<ToaNhaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(
        LayCauTrucChungCuSpecification spec,
        CancellationToken cancellationToken = default);
}
