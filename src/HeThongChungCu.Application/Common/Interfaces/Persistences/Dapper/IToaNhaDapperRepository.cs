using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IToaNhaDapperRepository
{
    Task<PagedResult<ToaNhaDetailResponse>> GetAllAsync(
        GetListToaNhaSpecification spec,
        CancellationToken cancellationToken = default);

    Task<ToaNhaResponse?> GetByIdAsync(GetToaNhaByIdSpecification spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(
        LayCauTrucChungCuSpecification spec,
        CancellationToken cancellationToken = default);
}
