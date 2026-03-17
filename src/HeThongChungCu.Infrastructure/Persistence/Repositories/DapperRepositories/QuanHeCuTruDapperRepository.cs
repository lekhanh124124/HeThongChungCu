using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
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
            { "CanHoId", "q.CanHoId" },
            { "IsKetThuc", "q.IsKetThuc" },
            { "NgayBatDau", "q.NgayBatDau" },
            { "IsDeleted", "q.IsDeleted" }

        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, nameof(QuanHeCuTru.NgayBatDau));

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
            { "CanHoId", "q.CanHoId" },
            { "UserId", "q.UserId" },
            { "NgayBatDau", "q.NgayBatDau" },
            { "NgayKetThuc", "q.NgayKetThuc" },
            { "IsKetThuc", "q.IsKetThuc" },
            { "LoaiQuanHeCuTruId", "q.LoaiQuanHeCuTruId" },
            { "MaCanHo", "c.MaCanHo" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, nameof(QuanHeCuTru.NgayBatDau));
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                q.Id         AS QuanHeCuTruId,
                q.CanHoId,
                c.MaCanHo,
                tg.ToaNhaId,
                t.TenToaNha,
                q.UserId,
                u.LastName + N' ' + u.FirstName AS HoTen,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.IsKetThuc
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN Tangs    tg ON tg.Id = c.TangId
            INNER JOIN ToaNhas  t ON t.Id = tg.ToaNhaId
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

    public async Task<IReadOnlyList<QuanHeCuTruResponse>> GetActiveByUserIdAsync(
        LayQuanHeCuTruSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "UserId", "q.UserId" },
            { "IsKetThuc", "q.IsKetThuc" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.Id,
                q.CanHoId,
                c.MaCanHo,
                c.TenCanHo,
                tg.ToaNhaId,
                t.MaToaNha,
                t.TenToaNha,
                q.LoaiQuanHeCuTruId,
                (SELECT COUNT(*) FROM QuanHeCuTrus qr WHERE qr.CanHoId = q.CanHoId AND qr.IsKetThuc = 0 AND qr.IsDeleted = 0) AS TongCuDan
            FROM QuanHeCuTrus q
            INNER JOIN CanHos   c ON c.Id = q.CanHoId
            INNER JOIN Tangs    tg ON tg.Id = c.TangId
            INNER JOIN ToaNhas  t ON t.Id = tg.ToaNhaId
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<GetActiveQuanHeCuTruReadModel>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new QuanHeCuTruResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            MaCanHo = r.MaCanHo,
            TenCanHo = r.TenCanHo,
            ToaNhaId = r.ToaNhaId,
            MaToaNha = r.MaToaNha,
            TenToaNha = r.TenToaNha,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            TongCuDan = r.TongCuDan,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty)
        }).ToList();

        return items;
    }

    public async Task<LayThongTinCuDanResponse?> GetByIdAsync(
        LayThongTinCuDanSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "q.Id" },
            { "UserId", "q.UserId" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.UserId,
                u.LastName + N' ' + u.FirstName AS FullName,
                u.PhoneNumber,
                u.IdCard,
                u.Dob,
                u.GioiTinhId,
                u.RoleId,
                u.AnhDaiDienUrl,
                q.Id             AS QuanHeCuTruId,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau
            FROM QuanHeCuTrus q
            INNER JOIN Users u ON u.Id = q.UserId
            {sqlWhere}
            """;

        var row = await connection.QueryFirstOrDefaultAsync<LayThongTinCuDanReadModel>(sql, parameters);

        if (row is null) return null;

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var gioiTinhMap = GioiTinh.ToDictionary();
        var roleMap = Role.ToDictionary();

        return new LayThongTinCuDanResponse
        {
            UserId = row.UserId,
            FullName = row.FullName,
            PhoneNumber = row.PhoneNumber,
            IdCard = row.IdCard,
            Dob = row.Dob,
            GioiTinhId = row.GioiTinhId,
            GioiTinhName = gioiTinhMap.GetValueOrDefault(row.GioiTinhId, string.Empty),
            RoleId = row.RoleId,
            RoleName = roleMap.GetValueOrDefault(row.RoleId, string.Empty),
            AnhDaiDienUrl = row.AnhDaiDienUrl,
            QuanHeCuTruId = row.QuanHeCuTruId,
            LoaiQuanHeCuTruId = row.LoaiQuanHeCuTruId,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(row.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = row.NgayBatDau
        };
    }
}
