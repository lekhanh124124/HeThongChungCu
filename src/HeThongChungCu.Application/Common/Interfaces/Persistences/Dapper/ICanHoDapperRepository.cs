using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface ICanHoDapperRepository
{
    Task<(int TotalCount, IReadOnlyList<CanHoDetailResponse> Items)> GetAllAsync(
        int? toaNhaId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<CanHoDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
