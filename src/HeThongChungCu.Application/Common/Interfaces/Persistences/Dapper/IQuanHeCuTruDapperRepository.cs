using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IQuanHeCuTruDapperRepository
{
    Task<IReadOnlyList<CuDanResponse>> GetCuDanByCanHoIdAsync(
        int canHoId,
        CancellationToken cancellationToken = default);

    Task<(int TotalCount, IReadOnlyList<LichSuCuTruResponse> Items)> GetLichSuByCanHoIdAsync(
        int canHoId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<(int TotalCount, IReadOnlyList<LichSuCuTruResponse> Items)> GetLichSuByUserIdAsync(
        int userId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LayQuanHeCuTruResponse>> GetActiveByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);
}
