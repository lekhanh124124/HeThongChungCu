using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class CanHoDapperRepository : ICanHoDapperRepository
{
    private readonly AppDbContext _dbContext;
    public CanHoDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TotalCount, IReadOnlyList<CanHoDetailResponse> Items)> GetAllAsync(
        int? tangId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "MaCanHo", "DienTich", "SoPhongNgu", "SoPhongTam", "TinhTrangCanHoId"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = (pageNumber - 1) * pageSize;

        var sql = $"""
            SELECT COUNT(*)
            FROM CanHos c
            WHERE c.IsDeleted = 0
              AND (@TangId IS NULL OR c.TangId = @TangId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%');

            SELECT c.Id, c.TangId, t.TenTang, c.MaCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            WHERE c.IsDeleted = 0
              AND (@TangId IS NULL OR c.TangId = @TangId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%')
            ORDER BY c.{orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            TangId = tangId,
            Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            Offset = offset,
            PageSize = pageSize
        };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<CanHoDetailResponse>();

        var result = items.ToList();
        foreach (var item in result)
        {
            item.TenLoaiCanHo = LoaiCanHo.FromValue(item.LoaiCanHoId)?.Name ?? string.Empty;
            item.TenTinhTrangCanHo = TinhTrangCanHo.FromValue(item.TinhTrangCanHoId)?.Name ?? string.Empty;
        }

        return (totalCount, result);
    }

    public async Task<CanHoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = """
            SELECT c.Id, c.TangId, t.TenTang, c.MaCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            WHERE c.Id = @Id AND c.IsDeleted = 0;

            SELECT q.Id, q.CanHoId, q.UserId, u.LastName + ' ' + u.FirstName AS FullName, q.LoaiQuanHeCuTruId, q.NgayBatDau, q.NgayKetThuc, q.IsKetThuc
            FROM QuanHeCuTrus q
            INNER JOIN Users u ON u.Id = q.UserId
            WHERE q.CanHoId = @Id AND q.IsDeleted = 0;
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
        var canHo = await multi.ReadFirstOrDefaultAsync<CanHoResponse>();

        if (canHo is null)
            return null;

        var quanHeCuTrus = (await multi.ReadAsync<QuanHeCuTruDetailResponse>()).ToList();

        // Map SmartEnums for QuanHeCuTru
        foreach (var q in quanHeCuTrus)
        {
            var lq = LoaiQuanHeCuTru.FromValue(q.LoaiQuanHeCuTruId);
            if (lq != null)
            {
                q.TenLoaiQuanHeCuTru = lq.Name;
            }
        }

        canHo.QuanHeCuTrus = quanHeCuTrus;

        // Map SmartEnums for CanHo
        var lc = LoaiCanHo.FromValue(canHo.LoaiCanHoId);
        if (lc != null)
        {
            canHo.TenLoaiCanHo = lc.Name;
        }

        var tc = TinhTrangCanHo.FromValue(canHo.TinhTrangCanHoId);
        if (tc != null)
        {
            canHo.TenTinhTrangCanHo = tc.Name;
        }

        return canHo;
    }
}
