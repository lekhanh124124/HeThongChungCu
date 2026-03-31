using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using System.Data;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class YeuCauCuTruDapperRepository : IYeuCauCuTruDapperRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauCuTruDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DSYeuCauCuTruResponse>> GetPagedListAsync(
        LayDSYeuCauCuTruQuerySpecification spec,
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

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                y.LoaiYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTen,
                y.YeuCauHo,
                y.YeuCauNgaySinh,
                y.YeuCauGioiTinhId,
                y.YeuCauSoDienThoai,
                y.YeuCauCCCD,
                y.YeuCauDiaChi,
                y.YeuCauLoaiQuanHeId,
                y.YeuCauQuanHeCuTruId,
                ch.TenCanHo,
                tg.TenTang,
                tg.ToaNhaId,
                ch.TangId,
                tn.TenToaNha,
                COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10))) AS TenNguoiGui,
                COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10))) AS TenNguoiXuLy
            FROM YeuCauCuTru y
            LEFT JOIN CanHo ch ON y.CanHoId = ch.Id
            LEFT JOIN Tang tg ON ch.TangId = tg.Id
            LEFT JOIN ToaNha tn ON tg.ToaNhaId = tn.Id
            LEFT JOIN NguoiDung nd1 ON y.CreatedBy = nd1.Id
            LEFT JOIN TaiKhoan tk1 ON nd1.Id = tk1.NguoiDungId
            LEFT JOIN NguoiDung nd2 ON y.NguoiXuLyId = nd2.Id
            LEFT JOIN TaiKhoan tk2 ON nd2.Id = tk2.NguoiDungId
            {(string.IsNullOrEmpty(sqlWhere) ? "" : sqlWhere)}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<YeuCauCuTruReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        var items = rows.Select(r => new DSYeuCauCuTruResponse
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
            TenNguoiXuLy = r.TenNguoiXuLy
        }).ToList();

        return new PagedResult<DSYeuCauCuTruResponse>
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

    public async Task<YeuCauCuTruResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id, y.CanHoId, y.LoaiYeuCauId, y.TrangThaiId, y.LyDo, y.NoiDung, 
                y.CreatedAt, y.NgayXuLy, y.NguoiXuLyId, y.CreatedBy,
                y.YeuCauTen, y.YeuCauHo, y.YeuCauNgaySinh, y.YeuCauGioiTinhId,
                y.YeuCauSoDienThoai, y.YeuCauCCCD, y.YeuCauDiaChi,
                y.YeuCauLoaiQuanHeId, y.YeuCauQuanHeCuTruId AS TargetQuanHeCuTruId,
                ch.TenCanHo, tg.TenTang, tn.TenToaNha,
                COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10))) AS TenNguoiGui,
                COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10))) AS TenNguoiXuLy
            FROM YeuCauCuTru y
            LEFT JOIN CanHo ch ON y.CanHoId = ch.Id
            LEFT JOIN Tang tg ON ch.TangId = tg.Id
            LEFT JOIN ToaNha tn ON tg.ToaNhaId = tn.Id
            LEFT JOIN NguoiDung nd1 ON y.CreatedBy = nd1.Id
            LEFT JOIN TaiKhoan tk1 ON nd1.Id = tk1.NguoiDungId
            LEFT JOIN NguoiDung nd2 ON y.NguoiXuLyId = nd2.Id
            LEFT JOIN TaiKhoan tk2 ON nd2.Id = tk2.NguoiDungId
            WHERE y.Id = @Id AND y.IsDeleted = 0;

            -- 2. Documents
            SELECT 
                ytl.Id, ytl.LoaiGiayToId, ytl.SoGiayTo, ytl.NgayPhatHanh, 
                ytl.TaiLieuCuTruId AS TargetTaiLieuCuTruId
            FROM YeuCauTaiLieuCuTru ytl
            WHERE ytl.YeuCauCuTruId = @Id;

            -- 3. Files
            SELECT 
                ttl.Id, ttl.FileUrl, ttl.FileName, ttl.ContentType,
                j.YeuCauTaiLieuCuTruId AS DocumentId
            FROM TepTaiLieu ttl
            JOIN TepYeuCauTaiLieuCuTru j ON ttl.Id = j.FilesId
            WHERE j.YeuCauTaiLieuCuTruId IN (SELECT Id FROM YeuCauTaiLieuCuTru WHERE YeuCauCuTruId = @Id);
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });

        var response = await multi.ReadFirstOrDefaultAsync<YeuCauCuTruResponse>();
        if (response == null) return null;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();
        var loaiGiayToMap = LoaiGiayTo.ToDictionary();

        // Enrich main info
        response = response with
        {
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(response.LoaiYeuCauId, string.Empty),
            TenTrangThai = trangThaiMap.GetValueOrDefault(response.TrangThaiId, string.Empty),
        };

        var documents = (await multi.ReadAsync<TaiLieuResponse>()).ToList();
        var fileRows = (await multi.ReadAsync<dynamic>()).ToList();

        // Map Enums and Stitch Files
        foreach (var doc in documents)
        {
            doc.TenLoaiGiayTo = loaiGiayToMap.GetValueOrDefault(doc.LoaiGiayToId, string.Empty);
            doc.Files = fileRows
                .Where(f => (int)f.DocumentId == doc.Id)
                .Select(f => new TepTaiLieuResponse((int)f.Id, (string)f.FileUrl, (string)f.FileName, (string)f.ContentType))
                .ToList();
        }

        return response with { Documents = documents };
    }
}
