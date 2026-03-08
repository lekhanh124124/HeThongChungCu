using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class ToaNhaDapperRepository : DapperDbContext, IToaNhaDapperRepository
{

    public ToaNhaDapperRepository(IConfiguration configuration)
        : base(configuration)
    {
    }

    public async Task<(int TotalCount, IReadOnlyList<ToaNhaDetailResponse> Items)> GetAllAsync(
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id", "MaToaNha", "TenToaNha", "SoTang"
            };

            var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
            var sortDirection = "ASC";
            if (isAsc.HasValue && !isAsc.Value)
            {
                sortDirection = "DESC";
            }
            var offset = (pageNumber - 1) * pageSize;

            var countSql = """
            SELECT COUNT(*)
            FROM ToaNhas
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR MaToaNha LIKE '%' + @Keyword + '%' OR TenToaNha LIKE '%' + @Keyword + '%')
            """;

            var dataSql = $"""
            SELECT Id, MaToaNha, TenToaNha, SoTang
            FROM ToaNhas
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR MaToaNha LIKE '%' + @Keyword + '%' OR TenToaNha LIKE '%' + @Keyword + '%')
            ORDER BY {orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

            var parameters = new
            {
                Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
                Offset = offset,
                PageSize = pageSize
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<ToaNhaDetailResponse>(dataSql, parameters);

            return (totalCount, items.ToList());
        }
    }

    public async Task<ToaNhaDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            const string sql = """
            SELECT Id, MaToaNha, TenToaNha, SoTang
            FROM ToaNhas
            WHERE Id = @Id AND IsDeleted = 0
            """;

            return await connection.QueryFirstOrDefaultAsync<ToaNhaDetailResponse>(sql, new { Id = id });
        }
    }
}
