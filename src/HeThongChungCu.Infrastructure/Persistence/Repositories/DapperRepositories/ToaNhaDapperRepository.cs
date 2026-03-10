using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class ToaNhaDapperRepository : IToaNhaDapperRepository
{
    private readonly DapperDbContext _context;
    public ToaNhaDapperRepository(DapperDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, IReadOnlyList<ToaNhaDetailResponse> Items)> GetAllAsync(
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
            "Id", "MaToaNha", "TenToaNha", "SoTang"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = (pageNumber - 1) * pageSize;

        var sql = $"""
            SELECT COUNT(*)
            FROM ToaNhas
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR MaToaNha LIKE '%' + @Keyword + '%' OR TenToaNha LIKE '%' + @Keyword + '%');

            SELECT Id, MaToaNha, TenToaNha, SoTang, SoTangHam, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHos WHERE ToaNhaId = ToaNhas.Id AND IsDeleted = 0) AS SoCanHo
            FROM ToaNhas
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR MaToaNha LIKE '%' + @Keyword + '%' OR TenToaNha LIKE '%' + @Keyword + '%')
            ORDER BY {orderColumn} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            Offset = offset,
            PageSize = pageSize
        };

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<ToaNhaDetailResponse>()).ToList();

        foreach (var item in items)
        {
            item.TenTrangThaiToaNha = TrangThaiToaNha.FromValue(item.TrangThaiToaNhaId)?.Name ?? string.Empty;
        }

        return (totalCount, items);
    }

    public async Task<ToaNhaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using (var connection = _context.CreateConnection())
        {
            const string sql = """
            SELECT Id, MaToaNha, TenToaNha, SoTang, SoTangHam, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHos WHERE ToaNhaId = ToaNhas.Id AND IsDeleted = 0) AS SoCanHo
            FROM ToaNhas
            WHERE Id = @Id AND IsDeleted = 0;

            SELECT Id, ToaNhaId, MaCanHo, Tang, DienTich, SoPhongNgu, SoPhongTam, LoaiCanHoId, TinhTrangCanHoId
            FROM CanHos
            WHERE ToaNhaId = @Id AND IsDeleted = 0;
            """;

            using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
            var toaNha = await multi.ReadFirstOrDefaultAsync<ToaNhaResponse>();

            if (toaNha != null)
            {
                toaNha.TenTrangThaiToaNha = TrangThaiToaNha.FromValue(toaNha.TrangThaiToaNhaId)?.Name ?? string.Empty;

                var canHos = (await multi.ReadAsync<CanHoDetailResponse>()).ToList();
                foreach (var canHo in canHos)
                {
                    canHo.TenLoaiCanHo = LoaiCanHo.FromValue(canHo.LoaiCanHoId)?.Name ?? string.Empty;
                    canHo.TenTinhTrangCanHo = TinhTrangCanHo.FromValue(canHo.TinhTrangCanHoId)?.Name ?? string.Empty;
                }
                toaNha.CanHos = canHos;
            }

            return toaNha;
        }
    }
}
