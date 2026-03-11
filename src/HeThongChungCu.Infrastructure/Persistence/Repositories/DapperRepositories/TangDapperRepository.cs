using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class TangDapperRepository : ITangDapperRepository
{
    private readonly AppDbContext _dbContext;

    public TangDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TotalCount, IReadOnlyList<TangDetailResponse> Items)> GetAllAsync(
        int? toaNhaId,
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
            "Id", "MaTang", "TenTang", "ToaNhaId", "LoaiTangId"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = ((pageNumber ?? 1) - 1) * (pageSize ?? 20);

        var sql = $"""
            SELECT COUNT(*)
            FROM Tangs t
            WHERE t.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR t.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR t.MaTang LIKE '%' + @Keyword + '%' OR t.TenTang LIKE '%' + @Keyword + '%');

            SELECT t.Id, t.MaTang, t.TenTang, t.LoaiTangId, t.ToaNhaId, tn.TenToaNha
            FROM Tangs t
            INNER JOIN ToaNhas tn ON tn.Id = t.ToaNhaId
            WHERE t.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR t.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR t.MaTang LIKE '%' + @Keyword + '%' OR t.TenTang LIKE '%' + @Keyword + '%')
            ORDER BY t.{orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            ToaNhaId = toaNhaId,
            Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            Offset = offset,
            PageSize = pageSize ?? 20
        };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<TangDetailResponse>();

        var result = items.ToList();
        foreach (var item in result)
        {
            item.TenLoaiTang = LoaiTang.FromValue(item.LoaiTangId)?.Name ?? string.Empty;
        }

        return (totalCount, result);
    }

    public async Task<TangResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = """
            SELECT t.Id, t.MaTang, t.TenTang, t.LoaiTangId, t.ToaNhaId, tn.TenToaNha
            FROM Tangs t
            INNER JOIN ToaNhas tn ON tn.Id = t.ToaNhaId
            WHERE t.Id = @Id AND t.IsDeleted = 0;

            SELECT c.Id, c.TangId, t.TenTang, c.MaCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            WHERE c.TangId = @Id AND c.IsDeleted = 0;
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
        var tang = await multi.ReadFirstOrDefaultAsync<TangResponse>();

        if (tang is null)
            return null;

        var canHos = (await multi.ReadAsync<CanHoDetailResponse>()).ToList();

        // Map SmartEnums for CanHo
        foreach (var c in canHos)
        {
            var lc = LoaiCanHo.FromValue(c.LoaiCanHoId);
            if (lc != null)
            {
                c.TenLoaiCanHo = lc.Name;
            }

            var tc = TinhTrangCanHo.FromValue(c.TinhTrangCanHoId);
            if (tc != null)
            {
                c.TenTinhTrangCanHo = tc.Name;
            }
        }

        tang.CanHos = canHos;

        // Map SmartEnums for Tang
        var lt = LoaiTang.FromValue(tang.LoaiTangId);
        if (lt != null)
        {
            tang.TenLoaiTang = lt.Name;
        }

        return tang;
    }
}
