using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using System.Data;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class QuanHeCuTruQueryRepository : IQuanHeCuTruQueryRepository
{
    private readonly AppDbContext _dbContext;
    public QuanHeCuTruQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CuDanResponse>> LayDSCuDanTrongChungCu(
        LayDSCuDanTrongChungCuQuerySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ToaNhaId", "tn.Id" },
            { "MaToaNha", "tn.MaToaNha" },
            { "TangId", "t.Id" },
            { "MaTang", "t.MaTang" },
            { "CanHoId", "q.CanHoId" },
            { "MaCanHo", "c.MaCanHo" },

            { "NguoiDungId", "q.NguoiDungId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "NgayBatDau", "q.NgayBatDau" },
            { "IsDeleted", "q.IsDeleted" },
            { "NgayKetThuc", "q.NgayKetThuc" },
            { "LoaiQuanHeCuTruId", "q.LoaiQuanHeCuTruId" },
            { "HoTen", "u.Ho + N' ' + u.Ten" },
            { "SoDienThoai", "u.SoDienThoai" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "NgayBatDau");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                tn.MaToaNha       AS MaToaNha,
                t.MaTang          AS MaTang,
                c.MaCanHo         AS MaCanHo,
                q.Id         AS QuanHeCuTruId,
                q.NguoiDungId,
                u.Ho + N' ' + u.Ten AS HoTen,
                u.SoDienThoai as PhoneNumber,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.TrangThaiCuTruId
            FROM QuanHeCuTru q
            LEFT JOIN NguoiDung u ON u.Id = q.NguoiDungId AND u.IsDeleted = 0
            LEFT JOIN CanHo c ON c.Id = q.CanHoId AND c.IsDeleted = 0
            LEFT JOIN Tang t ON t.Id = c.TangId AND t.IsDeleted = 0
            LEFT JOIN ToaNha tn ON tn.Id = t.ToaNhaId AND tn.IsDeleted = 0
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<DSCuDanTrongChungCuReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var trangThaiMap = TrangThaiCuTru.ToDictionary();
        var items = rows.Select(r => new CuDanResponse
        {
            MaToaNha = r.MaToaNha,
            MaTang = r.MaTang,
            MaCanHo = r.MaCanHo,
            QuanHeCuTruId = r.QuanHeCuTruId,
            UserId = r.NguoiDungId,
            HoTen = r.HoTen,
            PhoneNumber = r.PhoneNumber,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            TrangThaiCuTruId = r.TrangThaiCuTruId,
            TenTrangThaiCuTru = trangThaiMap.GetValueOrDefault(r.TrangThaiCuTruId, string.Empty)
        }).ToList();

        return new PagedResult<CuDanResponse>
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


    public async Task<IReadOnlyList<QuanHeCuTruResponse>> LayDSCuTruByUserId(
        LayDSCuTruCuaNguoiDungSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NguoiDungId", "q.NguoiDungId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.Id,
                tn.Id AS ToaNhaId,
                tn.MaToaNha,
                tn.TenToaNha,
                t.Id AS TangId,
                t.MaTang,
                t.TenTang,
                c.Id AS CanHoId,
                c.MaCanHo,
                c.TenCanHo,
                q.LoaiQuanHeCuTruId,
                (SELECT COUNT(*) FROM QuanHeCuTru qr 
                    WHERE qr.CanHoId = q.CanHoId 
                    AND qr.TrangThaiCuTruId = 1
                    AND qr.IsDeleted = 0) AS TongCuDan
            FROM QuanHeCuTru q
            LEFT JOIN CanHo   c ON c.Id = q.CanHoId AND c.IsDeleted = 0
            LEFT JOIN Tang    t ON t.Id = c.TangId AND t.IsDeleted = 0
            LEFT JOIN ToaNha  tn ON tn.Id = t.ToaNhaId AND tn.IsDeleted = 0
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<LayDSCuTruByUserIdReadModel>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new QuanHeCuTruResponse
        {
            QuanHeCuTruId = r.Id,
            ToaNhaId = r.ToaNhaId,
            MaToaNha = r.MaToaNha,
            TenToaNha = r.TenToaNha,
            TangId = r.TangId,
            MaTang = r.MaTang,
            TenTang = r.TenTang,
            CanHoId = r.CanHoId,
            MaCanHo = r.MaCanHo,
            TenCanHo = r.TenCanHo,
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
            { "NguoiDungId", "q.NguoiDungId" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.NguoiDungId,
                u.Ho + N' ' + u.Ten AS FullName,
                u.Ho as LastName,
                u.Ten as FirstName,
                u.SoDienThoai as PhoneNumber,
                u.NgaySinh as Dob,
                u.GioiTinhId,
                u.CCCD as IdCard,
                atl.FileUrl as AnhDaiDienUrl,
                q.Id             AS QuanHeCuTruId,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.TrangThaiCuTruId,
                u.DiaChi,
                -- Document fields
                t.Id AS DocId, t.LoaiGiayToId, t.SoGiayTo, t.NgayPhatHanh,
                -- File fields
                f.Id AS FileId, f.FileUrl, f.FileName, f.ContentType
            FROM QuanHeCuTru q
            INNER JOIN NguoiDung u ON u.Id = q.NguoiDungId
            LEFT JOIN TaiKhoan a ON u.Id = a.NguoiDungId
            LEFT JOIN TepTaiLieu atl ON a.AnhDaiDienId = atl.Id AND atl.IsDeleted = 0
            LEFT JOIN TaiLieuNguoiDung t ON t.NguoiDungId = u.Id
            LEFT JOIN TepTaiLieuNguoiDung tj ON tj.TaiLieuNguoiDungId = t.Id
            LEFT JOIN TepTaiLieu f ON f.Id = tj.FilesId AND f.IsDeleted = 0
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<dynamic>(sql, parameters);

        LayThongTinCuDanResponse? result = null;
        var docLookup = new Dictionary<int, TaiLieuResponse>();

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var gioiTinhMap = GioiTinh.ToDictionary();
        var trangThaiCuTruMap = TrangThaiCuTru.ToDictionary();

        foreach (var row in rows)
        {
            result ??= new LayThongTinCuDanResponse
            {
                UserId = row.NguoiDungId,
                FullName = row.FullName,
                FirstName = row.FirstName,
                LastName = row.LastName,
                PhoneNumber = row.PhoneNumber,
                Dob = row.Dob,
                GioiTinhId = row.GioiTinhId,
                GioiTinhName = gioiTinhMap.GetValueOrDefault((int)row.GioiTinhId, string.Empty),
                AnhDaiDienUrl = row.AnhDaiDienUrl ?? string.Empty,
                QuanHeCuTruId = row.QuanHeCuTruId,
                LoaiQuanHeCuTruId = row.LoaiQuanHeCuTruId,
                LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault((int)row.LoaiQuanHeCuTruId, string.Empty),
                NgayBatDau = row.NgayBatDau,
                NgayKetThuc = row.NgayKetThuc,
                TrangThaiCuTruId = row.TrangThaiCuTruId,
                TrangThaiCuTruTen = trangThaiCuTruMap.GetValueOrDefault((int)row.TrangThaiCuTruId, string.Empty),
                DiaChi = row.DiaChi,
                IdCard = row.IdCard,
                TaiLieuCuTrus = []
            };

            if (row.DocId != null)
            {
                if (!docLookup.TryGetValue((int)row.DocId, out var doc))
                {
                    doc = new TaiLieuResponse
                    {
                        Id = row.DocId,
                        LoaiGiayToId = row.LoaiGiayToId,
                        TenLoaiGiayTo = LoaiGiayTo.FromValue((int)row.LoaiGiayToId)?.Name ?? string.Empty,
                        SoGiayTo = row.SoGiayTo,
                        NgayPhatHanh = row.NgayPhatHanh,
                        Files = []
                    };
                    docLookup.Add(doc.Id, doc);
                    result.TaiLieuCuTrus.Add(doc);
                }

                if (row.FileId != null)
                {
                    if (!doc.Files.Any(f => f.Id == (int)row.FileId))
                    {
                        doc.Files.Add(new TepTaiLieuResponse(
                            (int)row.FileId,
                            (string)row.FileUrl,
                            (string)row.FileName,
                            (string)row.ContentType));
                    }
                }
            }
        }

        return result;
    }

    public async Task<IReadOnlyList<ThanhVienCuTruResponse>> LayThanhVienCuTru(
        LayThanhVienCuTruSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CanHoId", "q.CanHoId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "IsDeleted", "q.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT
                q.Id,
                u.Id as UserId,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                u.Ten as FirstName,
                u.Ho as LastName,
                atl.FileUrl as AnhDaiDienUrl
            FROM QuanHeCuTru q
            INNER JOIN NguoiDung u ON u.Id = q.NguoiDungId AND u.IsDeleted = 0
            LEFT JOIN TaiKhoan a ON u.Id = a.NguoiDungId
            LEFT JOIN TepTaiLieu atl ON a.AnhDaiDienId = atl.Id AND atl.IsDeleted = 0
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<ThanhVienCuTruReadModel>(sql, parameters);

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new ThanhVienCuTruResponse
        {
            QuanHeCuTruId = r.Id,
            UserId = r.UserId,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            FullName = $"{r.LastName} {r.FirstName}",
            AnhDaiDienUrl = r.AnhDaiDienUrl ?? string.Empty
        }).ToList();

        return items;
    }
}
