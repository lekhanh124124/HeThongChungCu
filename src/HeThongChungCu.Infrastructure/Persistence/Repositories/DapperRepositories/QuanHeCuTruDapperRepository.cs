using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class QuanHeCuTruDapperRepository : IQuanHeCuTruDapperRepository
{
    private readonly AppDbContext _dbContext;
    public QuanHeCuTruDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CuDanResponse>> GetCuDanByCanHoIdAsync(
        int canHoId,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        const string sql = """
            SELECT
                q.Id         AS QuanHeCuTruId,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                u.Email,
                u.PhoneNumber,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau
            FROM QuanHeCuTrus q
            INNER JOIN Users u ON u.Id = q.UserId
            WHERE q.CanHoId = @CanHoId
              AND q.IsKetThuc = 0
            ORDER BY q.NgayBatDau
            """;

        var items = await connection.QueryAsync<CuDanResponse>(sql, new { CanHoId = canHoId });

        // Enrich LoaiQuanHeTen in-memory (static enum lookup, no extra DB call)
        var result = items.Select(c =>
        {
            c.LoaiQuanHeTen = LoaiQuanHeCuTru.GetAll()
                .FirstOrDefault(l => l.Value == c.LoaiQuanHeCuTruId)?.Name ?? string.Empty;
            return c;
        }).ToList();

        return result;
    }

    public async Task<(int TotalCount, IReadOnlyList<LichSuCuTruResponse> Items)> GetLichSuByCanHoIdAsync(
        int canHoId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        var offset = (pageNumber - 1) * pageSize;
        var (orderColumn, sortDirection) = ResolveSortParams(sortCol, isAsc);

        var sql = $"""
            SELECT COUNT(*)
            FROM QuanHeCuTrus q
            WHERE q.CanHoId = @CanHoId;

            SELECT
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                c.ToaNhaId,
                t.TenToaNha,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.IsKetThuc
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN ToaNhas  t ON t.Id = c.ToaNhaId
            INNER JOIN Users    u ON u.Id = q.UserId
            WHERE q.CanHoId = @CanHoId
            ORDER BY {orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new { CanHoId = canHoId, Offset = offset, PageSize = pageSize };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<LichSuCuTruResponse>();

        return (totalCount, EnrichLoaiQuanHe(items.ToList()));
    }

    public async Task<(int TotalCount, IReadOnlyList<LichSuCuTruResponse> Items)> GetLichSuByUserIdAsync(
        int userId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        var offset = (pageNumber - 1) * pageSize;
        var (orderColumn, sortDirection) = ResolveSortParams(sortCol, isAsc);

        var sql = $"""
            SELECT COUNT(*)
            FROM QuanHeCuTrus q
            WHERE q.UserId = @UserId;

            SELECT
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                c.ToaNhaId,
                t.TenToaNha,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.IsKetThuc
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN ToaNhas  t ON t.Id = c.ToaNhaId
            INNER JOIN Users    u ON u.Id = q.UserId
            WHERE q.UserId = @UserId
            ORDER BY {orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new { UserId = userId, Offset = offset, PageSize = pageSize };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<LichSuCuTruResponse>();

        return (totalCount, EnrichLoaiQuanHe(items.ToList()));
    }

    private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "NgayBatDau", "NgayKetThuc", "MaCanHo", "IsKetThuc", "LoaiQuanHeCuTruId"
    };

    private static (string Column, string Direction) ResolveSortParams(string? sortCol, bool? isAsc)
    {
        var column = AllowedSortColumns.Contains(sortCol ?? string.Empty) ? $"q.{sortCol}" : "q.NgayBatDau";
        var direction = (isAsc.HasValue && isAsc.Value) ? "ASC" : "DESC";
        return (column, direction);
    }

    private static IReadOnlyList<LichSuCuTruResponse> EnrichLoaiQuanHe(List<LichSuCuTruResponse> items)
    {
        foreach (var item in items)
        {
            item.LoaiQuanHeTen = LoaiQuanHeCuTru.GetAll()
                .FirstOrDefault(l => l.Value == item.LoaiQuanHeCuTruId)?.Name ?? string.Empty;
        }
        return items;
    }

    public async Task<IReadOnlyList<LayQuanHeCuTruResponse>> GetActiveByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        const string sql = """
            SELECT
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                c.ToaNhaId,
                t.TenToaNha,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.IsKetThuc,
                c.DienTich,
                c.Tang
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN ToaNhas  t ON t.Id = c.ToaNhaId
            WHERE q.UserId = @UserId
              AND q.IsKetThuc = 0;
            """;

        var items = await connection.QueryAsync<LayQuanHeCuTruResponse>(sql, new { UserId = userId });

        // Enrich LoaiQuanHeTen
        var result = items.Select(item =>
        {
            item.LoaiQuanHeTen = LoaiQuanHeCuTru.GetAll()
                .FirstOrDefault(l => l.Value == item.LoaiQuanHeCuTruId)?.Name ?? string.Empty;
            return item;
        }).ToList();

        return result;
    }
}
