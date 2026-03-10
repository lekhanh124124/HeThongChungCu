using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class CanHoDapperRepository : ICanHoDapperRepository
{
    private readonly DapperDbContext _context;
    public CanHoDapperRepository(DapperDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, IReadOnlyList<CanHoDetailResponse> Items)> GetAllAsync(
        int? toaNhaId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "MaCanHo", "DienTich", "Tang", "SoPhongNgu", "SoPhongTam", "TinhTrangCanHoId"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = (pageNumber - 1) * pageSize;

        var sql = $"""
            SELECT COUNT(*)
            FROM CanHos c
            WHERE c.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR c.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%');

            SELECT c.Id, c.ToaNhaId, t.TenToaNha, c.MaCanHo, c.DienTich, c.Tang, c.SoPhongNgu, c.SoPhongTam, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN ToaNhas t ON t.Id = c.ToaNhaId
            WHERE c.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR c.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%')
            ORDER BY c.{orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            ToaNhaId = toaNhaId,
            Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            Offset = offset,
            PageSize = pageSize
        };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<CanHoDetailResponse>();

        return (totalCount, items.ToList());
    }

    public async Task<CanHoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT c.Id, c.ToaNhaId, c.MaCanHo, c.DienTich, c.Tang, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM CanHos c
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
