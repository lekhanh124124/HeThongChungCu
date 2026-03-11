using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IToaNhaDapperRepository
{
    Task<(int TotalCount, IReadOnlyList<ToaNhaDetailResponse> Items)> GetAllAsync(
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<ToaNhaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(string? keyword, CancellationToken cancellationToken = default);
}
