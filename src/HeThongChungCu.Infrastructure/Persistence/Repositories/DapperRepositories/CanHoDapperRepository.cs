using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class CanHoDapperRepository : DapperDbContext, ICanHoDapperRepository
{
    public CanHoDapperRepository(IConfiguration configuration)
        : base(configuration)
    {
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
        using var connection = CreateConnection();

        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "MaCanHo", "DienTich", "Tang", "SoPhongNgu", "SoPhongTam", "TinhTrangCanHoId"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = (pageNumber - 1) * pageSize;

        var countSql = """
            SELECT COUNT(*)
            FROM CanHos c
            WHERE c.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR c.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%')
            """;

        var dataSql = $"""
            SELECT c.Id, c.ToaNhaId, t.TenToaNha, c.MaCanHo, c.DienTich, c.Tang, c.SoPhongNgu, c.SoPhongTam, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN ToaNhas t ON t.Id = c.ToaNhaId
            WHERE c.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR c.ToaNhaId = @ToaNhaId)
              AND (@Keyword IS NULL OR c.MaCanHo LIKE '%' + @Keyword + '%')
            ORDER BY c.{orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var parameters = new
        {
            ToaNhaId = toaNhaId,
            Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            Offset = offset,
            PageSize = pageSize
        };

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<CanHoDetailResponse>(dataSql, parameters);

        return (totalCount, items.ToList());
    }

    public async Task<CanHoDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        const string sql = """
            SELECT c.Id, c.ToaNhaId, t.TenToaNha, c.MaCanHo, c.DienTich, c.Tang, c.SoPhongNgu, c.SoPhongTam, c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN ToaNhas t ON t.Id = c.ToaNhaId
            WHERE c.Id = @Id AND c.IsDeleted = 0
            """;

        return await connection.QueryFirstOrDefaultAsync<CanHoDetailResponse>(sql, new { Id = id });
    }
}
