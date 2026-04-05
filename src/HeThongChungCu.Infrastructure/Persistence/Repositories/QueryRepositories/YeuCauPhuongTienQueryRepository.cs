using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using System.Data;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauPhuongTienQueryRepository : IYeuCauPhuongTienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauPhuongTienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DSYeuCauPhuongTienResponse>> GetPagedListAsync(
        LayDSYeuCauPhuongTienQuerySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "LoaiYeuCauId", "y.LoaiYeuCauId" },
            { "TrangThaiId", "y.TrangThaiId" },
            { "IsDeleted", "y.IsDeleted" },
            { "CreatedAt", "y.CreatedAt" },
            { "ToaNhaId", "tg.ToaNhaId" },
            { "TangId", "ch.TangId" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(
            spec, 
            columnMapping, 
            discriminator: ("y.LoaiYeuCauCuDan", "YeuCauPhuongTien"));

        var joins = new[]
        {
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id"),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id"),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id"),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id"),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", AddSoftDelete: false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id"),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", AddSoftDelete: false)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                y.YeuCauPhuongTienId,
                y.LoaiYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTenPhuongTien,
                y.YeuCauLoaiPhuongTienId,
                y.YeuCauBienSo,
                y.YeuCauMauXe,
                ch.TenCanHo,
                tg.TenTang,
                tg.ToaNhaId,
                ch.TangId,
                tn.TenToaNha,
                COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10))) AS TenNguoiGui,
                COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10))) AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<YeuCauPhuongTienReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        var items = rows.Select(r => new DSYeuCauPhuongTienResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            TenTang = r.TenTang,
            TenToaNha = r.TenToaNha,
            LoaiYeuCauId = r.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(r.LoaiYeuCauId, string.Empty),
            TrangThaiId = r.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(r.TrangThaiId, string.Empty),
            LyDo = r.LyDo,
            NoiDung = r.NoiDung,
            CreatedAt = r.CreatedAt,
            NgayXuLy = r.NgayXuLy,
            NguoiXuLyId = r.NguoiXuLyId,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui,
            TenNguoiXuLy = r.TenNguoiXuLy,
            YeuCauTenPhuongTien = r.YeuCauTenPhuongTien,
            YeuCauBienSo = r.YeuCauBienSo
        }).ToList();

        return new PagedResult<DSYeuCauPhuongTienResponse>
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

    public async Task<YeuCauPhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var joins = new[]
        {
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id"),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id"),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id"),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id"),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", AddSoftDelete: false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id"),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", AddSoftDelete: false)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id, y.CanHoId, y.YeuCauPhuongTienId, y.LoaiYeuCauId, y.TrangThaiId, y.LyDo, y.NoiDung, 
                y.CreatedAt, y.NgayXuLy, y.NguoiXuLyId, y.CreatedBy,
                y.YeuCauTenPhuongTien, y.YeuCauLoaiPhuongTienId, y.YeuCauBienSo, y.YeuCauMauXe,
                ch.TenCanHo, tg.TenTang, tn.TenToaNha,
                COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10))) AS TenNguoiGui,
                COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10))) AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            WHERE y.Id = @Id AND y.LoaiYeuCauCuDan = 'YeuCauPhuongTien';

            -- 2. Images
            SELECT 
                ttl.Id, ttl.FileUrl, ttl.FileName, ttl.ContentType
            FROM TepTaiLieu ttl
            WHERE ttl.YeuCauId = @Id AND ttl.LoaiTepTaiLieu = 'TepYeuCauPhuongTien' AND ttl.IsDeleted = 0;
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id }, transaction: transaction);

        var readModel = await multi.ReadFirstOrDefaultAsync<YeuCauPhuongTienReadModel>();
        if (readModel == null) return null;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();
        var loaiPhuongTienMap = LoaiPhuongTien.ToDictionary();

        var images = (await multi.ReadAsync<TepTaiLieuResponse>()).ToList();

        return new YeuCauPhuongTienResponse
        {
            Id = readModel.Id,
            CreatedBy = readModel.CreatedBy,
            TenNguoiGui = readModel.TenNguoiGui,
            CreatedAt = readModel.CreatedAt,
            CanHoId = readModel.CanHoId,
            TenCanHo = readModel.TenCanHo,
            TenTang = readModel.TenTang,
            TenToaNha = readModel.TenToaNha,
            NguoiXuLyId = readModel.NguoiXuLyId,
            TenNguoiXuLy = readModel.TenNguoiXuLy,
            NgayXuLy = readModel.NgayXuLy,
            PhuongTienId = readModel.YeuCauPhuongTienId,
            LoaiYeuCauId = readModel.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(readModel.LoaiYeuCauId, string.Empty),
            TrangThaiId = readModel.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(readModel.TrangThaiId, string.Empty),
            NoiDung = readModel.NoiDung,
            LyDo = readModel.LyDo,
            YeuCauTenPhuongTien = readModel.YeuCauTenPhuongTien,
            YeuCauLoaiPhuongTienId = readModel.YeuCauLoaiPhuongTienId,
            TenYeuCauLoaiPhuongTien = loaiPhuongTienMap.GetValueOrDefault(readModel.YeuCauLoaiPhuongTienId, string.Empty),
            YeuCauBienSo = readModel.YeuCauBienSo,
            YeuCauMauXe = readModel.YeuCauMauXe,
            YeuCauHinhAnhPhuongTiens = images
        };
    }

    public async Task<DSYeuCauPhuongTienResponse?> GetListResponseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var joins = new[]
        {
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id"),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id"),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id"),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id"),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", AddSoftDelete: false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id"),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", AddSoftDelete: false)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT
                y.Id,
                y.CanHoId,
                y.YeuCauPhuongTienId,
                y.LoaiYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTenPhuongTien,
                y.YeuCauLoaiPhuongTienId,
                y.YeuCauBienSo,
                y.YeuCauMauXe,
                ch.TenCanHo,
                tg.TenTang,
                tg.ToaNhaId,
                ch.TangId,
                tn.TenToaNha,
                COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10))) AS TenNguoiGui,
                COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10))) AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            WHERE y.Id = @Id AND y.IsDeleted = 0 AND y.LoaiYeuCauCuDan = 'YeuCauPhuongTien'
            """;

        var transaction = _dbContext.GetDbTransaction();
        var row = await connection.QueryFirstOrDefaultAsync<YeuCauPhuongTienReadModel>(sql, new { Id = id }, transaction: transaction);
        if (row == null) return null;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        return new DSYeuCauPhuongTienResponse
        {
            Id = row.Id,
            CanHoId = row.CanHoId,
            TenCanHo = row.TenCanHo,
            TenTang = row.TenTang,
            TenToaNha = row.TenToaNha,
            LoaiYeuCauId = row.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(row.LoaiYeuCauId, string.Empty),
            TrangThaiId = row.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(row.TrangThaiId, string.Empty),
            LyDo = row.LyDo,
            NoiDung = row.NoiDung,
            CreatedAt = row.CreatedAt,
            NgayXuLy = row.NgayXuLy,
            NguoiXuLyId = row.NguoiXuLyId,
            CreatedBy = row.CreatedBy,
            TenNguoiGui = row.TenNguoiGui,
            TenNguoiXuLy = row.TenNguoiXuLy,
            YeuCauTenPhuongTien = row.YeuCauTenPhuongTien,
            YeuCauBienSo = row.YeuCauBienSo
        };
    }
}
