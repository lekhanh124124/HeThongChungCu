using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;
using HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories.Helpers;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class QuanHeCuTruDapperRepository : IQuanHeCuTruDapperRepository
{
    private readonly AppDbContext _dbContext;
    public QuanHeCuTruDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CuDanResponse>> GetCuDanByCanHoIdAsync(
        LayCuDanByCanHoIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CanHoId", "q.CanHoId" },
            { "IsKetThuc", "q.IsKetThuc" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "q.NgayBatDau");

        var sql = $"""
            SELECT
                q.Id         AS QuanHeCuTruId,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                u.Email,
                u.PhoneNumber,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau
            FROM QuanHeCuTrus q
            INNER JOIN Users u ON u.Id = q.UserId
            {sqlWhere}
            {sqlOrderBy}
            """;

        var items = await connection.QueryAsync<CuDanResponse>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        foreach (var item in items)
        {
            item.TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(item.LoaiQuanHeCuTruId, string.Empty);
        }

        return items.ToList();
    }

    public async Task<PagedResult<LichSuCuTruResponse>> GetLichSuAsync(
        LayLichSuCuTruSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CanHoId", "q.CanHoId" },
            { "UserId", "q.UserId" },
            { "NgayBatDau", "q.NgayBatDau" },
            { "NgayKetThuc", "q.NgayKetThuc" },
            { "IsKetThuc", "q.IsKetThuc" },
            { "LoaiQuanHeCuTruId", "q.LoaiQuanHeCuTruId" },
            { "MaCanHo", "c.MaCanHo" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "q.NgayBatDau");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*)
            FROM QuanHeCuTrus q
            INNER JOIN CanHos c ON c.Id = q.CanHoId
            {sqlWhere};

            SELECT
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                c.ToaNhaId,
                t.TenToaNha,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.IsKetThuc
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN ToaNhas  t ON t.Id = c.ToaNhaId
            INNER JOIN Users    u ON u.Id = q.UserId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        var totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<LichSuCuTruResponse>()).ToList();
        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        foreach (var item in items)
        {
            item.LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(item.LoaiQuanHeCuTruId, string.Empty);
        }

        return new PagedResult<LichSuCuTruResponse>
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

    public async Task<IReadOnlyList<LayQuanHeCuTruResponse>> GetActiveByUserIdAsync(
        LayQuanHeCuTruSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "UserId", "q.UserId" },
            { "IsKetThuc", "q.IsKetThuc" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                c.ToaNhaId,
                t.TenToaNha,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.IsKetThuc,
                c.DienTich,
                c.Tang
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN ToaNhas  t ON t.Id = c.ToaNhaId
            {sqlWhere}
            """;

        var items = await connection.QueryAsync<LayQuanHeCuTruResponse>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        foreach (var item in items)
        {
            item.LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(item.LoaiQuanHeCuTruId, string.Empty);
        }

        return items.ToList();
    }
}
