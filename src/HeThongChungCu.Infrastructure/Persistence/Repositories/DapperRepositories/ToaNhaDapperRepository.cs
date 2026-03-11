using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class ToaNhaDapperRepository : IToaNhaDapperRepository
{
    private readonly AppDbContext _dbContext;
    public ToaNhaDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TotalCount, IReadOnlyList<ToaNhaDetailResponse> Items)> GetAllAsync(
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

        var transaction = _dbContext.GetDbTransaction();

        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "MaToaNha", "TenToaNha"
        };

        var orderColumn = allowedSortColumns.Contains(sortCol ?? "") ? sortCol : "Id";
        var sortDirection = (isAsc.HasValue && !isAsc.Value) ? "DESC" : "ASC";
        var offset = (pageNumber - 1) * pageSize;

        var sql = $"""
            SELECT COUNT(*)
            FROM ToaNhas
            WHERE IsDeleted = 0
              AND (@Keyword IS NULL OR MaToaNha LIKE '%' + @Keyword + '%' OR TenToaNha LIKE '%' + @Keyword + '%');

            SELECT Id, MaToaNha, TenToaNha, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHos c JOIN Tangs t ON c.TangId = t.Id WHERE t.ToaNhaId = ToaNhas.Id AND c.IsDeleted = 0 AND t.IsDeleted = 0) AS SoCanHo
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
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        const string sql = """
            SELECT Id, MaToaNha, TenToaNha, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHos c JOIN Tangs t ON c.TangId = t.Id WHERE t.ToaNhaId = ToaNhas.Id AND c.IsDeleted = 0 AND t.IsDeleted = 0) AS SoCanHo
            FROM ToaNhas
            WHERE Id = @Id AND IsDeleted = 0;

            SELECT c.Id, c.MaCanHo, c.TangId, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM CanHos c
            JOIN Tangs t ON c.TangId = t.Id
            WHERE t.ToaNhaId = @Id AND c.IsDeleted = 0;
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

    public async Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = """
            SELECT Id, MaToaNha, TenToaNha, TrangThaiToaNhaId AS TrangThaiId
            FROM ToaNhas
            WHERE IsDeleted = 0;

            SELECT Id, ToaNhaId, MaTang, TenTang, LoaiTangId
            FROM Tangs
            WHERE IsDeleted = 0;

            SELECT Id, TangId, MaCanHo, TinhTrangCanHoId
            FROM CanHos
            WHERE IsDeleted = 0;
            """;

        using var multi = await connection.QueryMultipleAsync(sql);
        var toaNhas = (await multi.ReadAsync<CauTrucToaNhaResponse>()).ToList();
        var allTangs = (await multi.ReadAsync<dynamic>()).ToList();
        var allCanHos = (await multi.ReadAsync<dynamic>()).ToList();

        var result = new List<CauTrucToaNhaResponse>();
        bool hasKeyword = !string.IsNullOrWhiteSpace(keyword);
        string k = hasKeyword ? keyword!.ToLower() : string.Empty;

        foreach (var toaNha in toaNhas)
        {
            toaNha.TenTrangThai = TrangThaiToaNha.FromValue(toaNha.TrangThaiId)?.Name ?? string.Empty;

            bool toaNhaMatch = hasKeyword && (toaNha.MaToaNha.ToLower().Contains(k) || toaNha.TenToaNha.ToLower().Contains(k));

            var tangsOfToaNha = allTangs.Where(t => (int)t.ToaNhaId == toaNha.Id).ToList();
            var cauTrucTangs = new List<CauTrucTangResponse>();

            foreach (var t in tangsOfToaNha)
            {
                bool tangMatch = toaNhaMatch || (hasKeyword && (((string)t.MaTang).ToLower().Contains(k) || ((string)t.TenTang).ToLower().Contains(k)));

                var canHosOfTang = allCanHos.Where(c => (int)c.TangId == (int)t.Id).ToList();
                var cauTrucCanHos = new List<CauTrucCanHoResponse>();

                foreach (var c in canHosOfTang)
                {
                    bool canHoMatch = tangMatch || (hasKeyword && (((string)c.MaCanHo).ToLower().Contains(k)));

                    if (!hasKeyword || canHoMatch)
                    {
                        cauTrucCanHos.Add(new CauTrucCanHoResponse
                        {
                            Id = (int)c.Id,
                            MaCanHo = (string)c.MaCanHo,
                            TenCanHo = (string)c.MaCanHo,
                            TrangThaiId = (int)c.TinhTrangCanHoId,
                            TenTrangThai = TinhTrangCanHo.FromValue((int)c.TinhTrangCanHoId)?.Name ?? string.Empty
                        });
                    }
                }

                if (!hasKeyword || tangMatch || cauTrucCanHos.Any())
                {
                    cauTrucTangs.Add(new CauTrucTangResponse
                    {
                        Id = (int)t.Id,
                        MaTang = (string)t.MaTang,
                        TenTang = (string)t.TenTang,
                        CauTrucCanHos = cauTrucCanHos.OrderBy(c => c.MaCanHo).ToList()
                    });
                }
            }

            if (!hasKeyword || toaNhaMatch || cauTrucTangs.Any())
            {
                toaNha.CauTrucTangs = cauTrucTangs.OrderBy(t => t.Id).ToList();
                result.Add(toaNha);
            }
        }

        return result.OrderBy(t => t.TenToaNha).ToList();
    }
}
