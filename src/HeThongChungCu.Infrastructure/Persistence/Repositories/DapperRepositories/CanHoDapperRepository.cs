using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.Helpers;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class CanHoDapperRepository : ICanHoDapperRepository
{
    private readonly AppDbContext _dbContext;
    public CanHoDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CanHoDetailResponse>> GetAllAsync(
        GetListCanHoSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "c.Id" },
            { "IsDeleted", "c.IsDeleted" },
            { "TangId", "c.TangId" },
            { "MaCanHo", "c.MaCanHo" },
            { "TenCanHo", "c.TenCanHo" },
            { "DienTich", "c.DienTich" },
            { "SoPhongNgu", "c.SoPhongNgu" },
            { "SoPhongTam", "c.SoPhongTam" },
            { "TinhTrangCanHoId", "c.TinhTrangCanHoId" },
            { "LoaiCanHoId", "c.LoaiCanHoId" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*)
            FROM CanHos c
            {sqlWhere};

            SELECT c.Id, c.TangId, t.TenTang, c.MaCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.TinhTrangCanHoId, c.LoaiCanHoId
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<CanHoDetailResponse>()).ToList();

        var loaiMap = LoaiCanHo.ToDictionary();
        var tinhTrangMap = TinhTrangCanHo.ToDictionary();

        foreach (var item in items)
        {
            item.TenLoaiCanHo = loaiMap.GetValueOrDefault(item.LoaiCanHoId, string.Empty);
            item.TenTinhTrangCanHo = tinhTrangMap.GetValueOrDefault(item.TinhTrangCanHoId, string.Empty);
        }

        return new PagedResult<CanHoDetailResponse>
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

    public async Task<CanHoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

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

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);
        var canHo = await multi.ReadFirstOrDefaultAsync<CanHoResponse>();

        if (canHo is null)
            return null;

        var quanHeCuTrus = (await multi.ReadAsync<QuanHeCuTruDetailResponse>()).ToList();
        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        foreach (var item in quanHeCuTrus)
        {
            item.TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(item.LoaiQuanHeCuTruId, string.Empty);
        }

        canHo.QuanHeCuTrus = quanHeCuTrus;
        canHo.TenLoaiCanHo = LoaiCanHo.ToDictionary().GetValueOrDefault(canHo.LoaiCanHoId, string.Empty);
        canHo.TenTinhTrangCanHo = TinhTrangCanHo.ToDictionary().GetValueOrDefault(canHo.TinhTrangCanHoId, string.Empty);

        return canHo;
    }
}
