using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.Helpers;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.ReadModels;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class ToaNhaDapperRepository : IToaNhaDapperRepository
{
    private readonly AppDbContext _dbContext;
    public ToaNhaDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ToaNhaDetailResponse>> GetAllAsync(
        GetListToaNhaSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "Id" },
            { "MaToaNha", "MaToaNha" },
            { "TenToaNha", "TenToaNha" },
            { "IsDeleted", "IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*)
            FROM ToaNhas
            {sqlWhere};

            SELECT Id, MaToaNha, TenToaNha, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHos c JOIN Tangs t ON c.TangId = t.Id WHERE t.ToaNhaId = ToaNhas.Id AND c.IsDeleted = 0 AND t.IsDeleted = 0) AS SoCanHo
            FROM ToaNhas
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<ToaNhaDetailResponse>()).ToList();
        var trangThaiToaNhaMap = TrangThaiToaNha.ToDictionary();

        foreach (var item in items)
        {
            item.TenTrangThaiToaNha = trangThaiToaNhaMap.GetValueOrDefault(item.TrangThaiToaNhaId, string.Empty);
        }

        return new PagedResult<ToaNhaDetailResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? items.Count,
                TotalItems = totalCount
            }
        };
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
            toaNha.TenTrangThaiToaNha = TrangThaiToaNha.ToDictionary().GetValueOrDefault(toaNha.TrangThaiToaNhaId, string.Empty);

            var canHos = (await multi.ReadAsync<CanHoDetailResponse>()).ToList();

            var loaiCanHoMap = LoaiCanHo.ToDictionary();
            var tinhTrangCanHoMap = TinhTrangCanHo.ToDictionary();
            foreach (var canHo in canHos)
            {
                canHo.TenLoaiCanHo = loaiCanHoMap.GetValueOrDefault(canHo.LoaiCanHoId, string.Empty);
                canHo.TenTinhTrangCanHo = tinhTrangCanHoMap.GetValueOrDefault(canHo.TinhTrangCanHoId, string.Empty);
            }
            toaNha.CanHos = canHos;
        }

        return toaNha;
    }

    public async Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(
        LayCauTrucChungCuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "MaToaNha", "t.MaToaNha" },
            { "TenToaNha", "t.TenToaNha" },
            { "MaTang", "f.MaTang" },
            { "TenTang", "f.TenTang" },
            { "MaCanHo", "c.MaCanHo" },
            { "TenCanHo", "c.TenCanHo" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT 
                t.Id AS ToaNhaId, t.MaToaNha, t.TenToaNha, t.TrangThaiToaNhaId AS ToaNhaTrangThaiId,
                f.Id AS TangId, f.MaTang, f.TenTang,
                c.Id AS CanHoId, c.MaCanHo, c.TinhTrangCanHoId AS CanHoTrangThaiId
            FROM ToaNhas t
            LEFT JOIN Tangs f ON f.ToaNhaId = t.Id AND f.IsDeleted = 0
            LEFT JOIN CanHos c ON c.TangId = f.Id AND c.IsDeleted = 0
            {sqlWhere}
            AND t.IsDeleted = 0
            ORDER BY t.TenToaNha, f.Id, c.MaCanHo
            """;

        var flatItems = await connection.QueryAsync<dynamic>(sql, parameters);

        var toaNhaMap = new Dictionary<int, CauTrucToaNhaResponse>();
        var tangMap = new Dictionary<int, CauTrucTangResponse>();

        var trangThaiToaNhaMap = TrangThaiToaNha.ToDictionary();
        var tinhTrangCanHoMap = TinhTrangCanHo.ToDictionary();

        foreach (var item in flatItems)
        {
            if (!toaNhaMap.TryGetValue((int)item.ToaNhaId, out var toaNha))
            {
                toaNha = new CauTrucToaNhaResponse
                {
                    Id = item.ToaNhaId,
                    MaToaNha = item.MaToaNha,
                    TenToaNha = item.TenToaNha,
                    TrangThaiId = item.ToaNhaTrangThaiId,
                    TenTrangThai = trangThaiToaNhaMap.GetValueOrDefault((int)item.ToaNhaTrangThaiId, string.Empty),
                    CauTrucTangs = new List<CauTrucTangResponse>()
                };
                toaNhaMap.Add(toaNha.Id, toaNha);
            }

            if (item.TangId != null)
            {
                if (!tangMap.TryGetValue((int)item.TangId, out var tang))
                {
                    tang = new CauTrucTangResponse
                    {
                        Id = item.TangId,
                        MaTang = item.MaTang,
                        TenTang = item.TenTang,
                        CauTrucCanHos = new List<CauTrucCanHoResponse>()
                    };
                    tangMap.Add(tang.Id, tang);
                    toaNha.CauTrucTangs.Add(tang);
                }

                if (item.CanHoId != null)
                {
                    tang.CauTrucCanHos.Add(new CauTrucCanHoResponse
                    {
                        Id = item.CanHoId,
                        MaCanHo = item.MaCanHo,
                        TenCanHo = item.MaCanHo, 
                        TrangThaiId = item.CanHoTrangThaiId,
                        TenTrangThai = tinhTrangCanHoMap.GetValueOrDefault((int)item.CanHoTrangThaiId, string.Empty)
                    });
                }
            }
        }

        return toaNhaMap.Values.ToList();
    }
}
