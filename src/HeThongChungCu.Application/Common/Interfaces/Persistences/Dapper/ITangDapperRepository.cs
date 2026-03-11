using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface ITangDapperRepository
{
    Task<(int TotalCount, IReadOnlyList<TangDetailResponse> Items)> GetAllAsync(
        int? toaNhaId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<TangResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
