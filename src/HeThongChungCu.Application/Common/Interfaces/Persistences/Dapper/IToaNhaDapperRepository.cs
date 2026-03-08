using HeThongChungCu.Application.Features.ChungCu.DTOs;

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

    Task<ToaNhaDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
