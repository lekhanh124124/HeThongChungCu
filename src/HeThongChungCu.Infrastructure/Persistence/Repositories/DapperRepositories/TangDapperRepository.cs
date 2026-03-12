using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.Helpers;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class TangDapperRepository : ITangDapperRepository
{
    private readonly AppDbContext _dbContext;

    public TangDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TangDetailResponse>> GetAllAsync(
        GetListTangSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "MaTang", "t.MaTang" },
            { "TenTang", "t.TenTang" },
            { "ToaNhaId", "t.ToaNhaId" },
            { "LoaiTangId", "t.LoaiTangId" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*)
            FROM Tangs t
            {sqlWhere};

            SELECT t.Id, t.MaTang, t.TenTang, t.LoaiTangId, t.ToaNhaId, tn.TenToaNha
            FROM Tangs t
            INNER JOIN ToaNhas tn ON tn.Id = t.ToaNhaId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<TangDetailResponse>();

        var result = items.ToList();
        var loaiTangMap = LoaiTang.ToDictionary();
        foreach (var item in result)
        {
            item.TenLoaiTang = loaiTangMap.GetValueOrDefault(item.LoaiTangId, string.Empty);
        }

        return new PagedResult<TangDetailResponse>
        {
            Items = result,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? result.Count,
                TotalItems = totalCount
            }
        };
    }

    public async Task<TangResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

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
        var loaiCanHoDict = LoaiCanHo.ToDictionary();
        var tinhTrangCanHoDict = TinhTrangCanHo.ToDictionary();

        foreach (var item in canHos)
        {
            item.TenLoaiCanHo = loaiCanHoDict.GetValueOrDefault(item.LoaiCanHoId, string.Empty);
            item.TenTinhTrangCanHo = loaiCanHoDict.GetValueOrDefault(item.TinhTrangCanHoId, string.Empty);
        }

        tang.CanHos = canHos;
        tang.TenLoaiTang = LoaiTang.ToDictionary().GetValueOrDefault(tang.LoaiTangId, string.Empty);

        return tang;
    }
}
