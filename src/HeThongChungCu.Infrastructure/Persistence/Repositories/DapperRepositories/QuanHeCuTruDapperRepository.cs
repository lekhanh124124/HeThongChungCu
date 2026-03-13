using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
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
            { nameof(QuanHeCuTru.CanHoId), "q.CanHoId" },
            { nameof(QuanHeCuTru.IsKetThuc), "q.IsKetThuc" }
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

        var rows = await connection.QueryAsync<GetCuDanByCanHoIdReadModel>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new CuDanResponse
        {
            QuanHeCuTruId = r.QuanHeCuTruId,
            UserId = r.UserId,
            HoTen = r.HoTen,
            Email = r.Email,
            PhoneNumber = r.PhoneNumber,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            NgayBatDau = r.NgayBatDau,
            TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty)
        }).ToList();

        return items;
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
            { nameof(QuanHeCuTru.CanHoId), "q.CanHoId" },
            { nameof(QuanHeCuTru.UserId), "q.UserId" },
            { nameof(QuanHeCuTru.NgayBatDau), "q.NgayBatDau" },
            { nameof(QuanHeCuTru.NgayKetThuc), "q.NgayKetThuc" },
            { nameof(QuanHeCuTru.IsKetThuc), "q.IsKetThuc" },
            { nameof(QuanHeCuTru.LoaiQuanHeCuTruId), "q.LoaiQuanHeCuTruId" },
            { nameof(CanHo.MaCanHo), "c.MaCanHo" },
            { nameof(QuanHeCuTru.IsDeleted), "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "q.NgayBatDau");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
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

        var rows = (await connection.QueryAsync<GetLichSuCuTruReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new LichSuCuTruResponse
        {
            QuanHeCuTruId = r.QuanHeCuTruId,
            CanHoId = r.CanHoId,
            MaCanHo = r.MaCanHo,
            ToaNhaId = r.ToaNhaId,
            TenToaNha = r.TenToaNha,
            UserId = r.UserId,
            HoTen = r.HoTen,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            IsKetThuc = r.IsKetThuc,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty)
        }).ToList();

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
            { nameof(QuanHeCuTru.CanHoId), "q.CanHoId" },
            { nameof(QuanHeCuTru.IsKetThuc), "q.IsKetThuc" },
            { nameof(QuanHeCuTru.IsDeleted), "q.IsDeleted" }
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

        var rows = await connection.QueryAsync<GetActiveQuanHeCuTruReadModel>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new LayQuanHeCuTruResponse
        {
            QuanHeCuTruId = r.QuanHeCuTruId,
            CanHoId = r.CanHoId,
            MaCanHo = r.MaCanHo,
            ToaNhaId = r.ToaNhaId,
            TenToaNha = r.TenToaNha,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            NgayBatDau = r.NgayBatDau,
            IsKetThuc = r.IsKetThuc,
            DienTich = r.DienTich,
            Tang = r.Tang,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty)
        }).ToList();

        return items;
    }
}
