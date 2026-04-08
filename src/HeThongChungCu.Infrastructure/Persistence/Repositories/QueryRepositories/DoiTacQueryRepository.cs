using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class DoiTacQueryRepository : IDoiTacQueryRepository
{
    private readonly AppDbContext _dbContext;

    public DoiTacQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DoiTacResponse>> GetAllAsync(
        HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac.GetListDoiTacSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "IsDeleted", "dt.IsDeleted" },
            { "TenDoiTac", "dt.TenDoiTac" },
            { "TenCongTy", "dt.TenCongTy" },
            { "Email", "dt.Email" },
            { "SoDienThoai", "dt.SoDienThoai" },
            { "LoaiDichVuId", "hd_filter.LoaiDichVuId" },
            { "Id", "dt.Id" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var joins = new List<JoinDefinition>();
        var loaiDichVuFilter = spec.Filters.FirstOrDefault(f => f.PropertyName == "LoaiDichVuId");
        if (loaiDichVuFilter != null)
        {
            joins.Add(new JoinDefinition("HopDongDoiTac", "hd_filter", "hd_filter.DoiTacId = dt.Id AND hd_filter.TrangThaiHopDongId = 1", JoinType.Inner, false));
        }
        var sqlJoin = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT COUNT(DISTINCT dt.Id) OVER() AS TotalCount, 
                   dt.Id, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien, dt.SoDienThoai, dt.Email,
                   (SELECT MAX(NgayHetHan) FROM HopDongDoiTac hd WHERE hd.DoiTacId = dt.Id AND hd.TrangThaiHopDongId = 1) AS NgayHetHan
            FROM DoiTac dt
            {sqlJoin}
            {sqlWhere}
            GROUP BY dt.Id, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien, dt.SoDienThoai, dt.Email
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DoiTacResponse
        {
            Id = (int)r.Id,
            TenDoiTac = (string)r.TenDoiTac,
            TenCongTy = (string?)r.TenCongTy,
            NguoiDaiDien = (string?)r.NguoiDaiDien,
            SoDienThoai = (string?)r.SoDienThoai,
            Email = (string?)r.Email,
            NgayHetHan = (DateTimeOffset?)r.NgayHetHan
        }).ToList();

        return new PagedResult<DoiTacResponse>
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

    public async Task<DoiTacDetailResponse?> GetByIdAsync(
        GetDoiTacByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dt.Id" },
            { "IsDeleted", "dt.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var joins = new List<JoinDefinition>
        {
            new("HopDongDoiTac", "hd", "hd.DoiTacId = dt.Id", JoinType.Left, false),
            new("DichVu", "dv", "dv.Id = hd.DichVuId", JoinType.Left, false),
            new("TepTaiLieu", "tp", "tp.HopDongDoiTacId = hd.Id", JoinType.Left, false)
        };
        var sqlJoin = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT dt.Id, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien, dt.SoGiayPhepKD, dt.MaSoThue, dt.DiaChi, dt.SoDienThoai, dt.Email,
                   dv.Id AS DichVuUid, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.IsBatBuoc, dv.TrangThaiId AS DichVuTrangThaiId,
                   hd.Id AS HopDongUid, hd.SoHopDong, hd.NgayKy, hd.NgayHetHan, hd.GiaTriHopDong_SoTien, hd.NoiDung, hd.DichVuId AS HopDongDichVuId, hd.TrangThaiHopDongId,
                   tp.Id AS FileUid, tp.FileUrl, tp.FileName, tp.ContentType
            FROM DoiTac dt
            {sqlJoin}
            {sqlWhere}
            ORDER BY hd.NgayKy DESC;
            """;

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();

        if (rows.Count == 0)
            return null;

        var firstRow = rows.First();

        var doiTac = new DoiTacDetailResponse
        {
            Id = (int)firstRow.Id,
            TenDoiTac = (string)firstRow.TenDoiTac,
            TenCongTy = (string?)firstRow.TenCongTy,
            NguoiDaiDien = (string?)firstRow.NguoiDaiDien,
            SoGiayPhepKD = (string?)firstRow.SoGiayPhepKD,
            MaSoThue = (string?)firstRow.MaSoThue,
            DiaChi = (string?)firstRow.DiaChi,
            SoDienThoai = (string?)firstRow.SoDienThoai,
            Email = (string?)firstRow.Email,
            HopDongs = []
        };

        var loaiDichVuMap = LoaiDichVu.ToDictionary();
        var trangThaiHopDongMap = TrangThaiHopDong.ToDictionary();
        var trangThaiDichVuMap = TrangThaiDichVu.ToDictionary();
        var fileIds = new HashSet<int>();

        foreach (var row in rows)
        {
            if (row.HopDongUid != null)
            {
                var hopDongId = (int)row.HopDongUid;
                var existingHopDong = doiTac.HopDongs.FirstOrDefault(h => h.Id == hopDongId);

                if (existingHopDong == null)
                {
                    existingHopDong = new HopDongResponse
                    {
                        Id = hopDongId,
                        SoHopDong = (string)row.SoHopDong,
                        NgayKy = (DateTimeOffset)row.NgayKy,
                        NgayHetHan = (DateTimeOffset)row.NgayHetHan,
                        GiaTriHopDong = (decimal)row.GiaTriHopDong_SoTien,
                        LoaiDichVuId = (int)row.HopDongDichVuId,
                        TenLoaiDichVu = loaiDichVuMap.GetValueOrDefault((int)row.HopDongDichVuId, string.Empty),
                        TrangThaiHopDongId = (int)row.TrangThaiHopDongId,
                        TenTrangThaiHopDong = trangThaiHopDongMap.GetValueOrDefault((int)row.TrangThaiHopDongId, string.Empty),
                        NoiDung = (string?)row.NoiDung,
                        Teps = [],

                        // Dich Vu Info
                        MaDichVu = (string?)row.MaDichVu ?? string.Empty,
                        TenDichVu = (string?)row.TenDichVu ?? string.Empty,
                        DonViTinh = (string?)row.DonViTinh ?? string.Empty,
                        IsBatBuoc = (bool?)row.IsBatBuoc ?? false,
                        TrangThaiDichVuId = (int?)row.DichVuTrangThaiId ?? 0,
                        TrangThaiDichVuTen = trangThaiDichVuMap.GetValueOrDefault((int?)row.DichVuTrangThaiId ?? 0, string.Empty)
                    };
                    doiTac.HopDongs.Add(existingHopDong);
                }

                if (row.FileUid != null && fileIds.Add((int)row.FileUid))
                {
                    existingHopDong.Teps.Add(new UploadFileResponse((int)row.FileUid, (string)row.FileName, (string)row.FileUrl, (string)row.ContentType));
                }
            }
        }

        // Set top-level contract dates for backward compatibility or easy access
        var latestHopDong = doiTac.HopDongs.OrderByDescending(h => h.NgayHetHan).FirstOrDefault();
        if (latestHopDong != null)
        {
            doiTac.NgayHetHan = latestHopDong.NgayHetHan;
        }

        return doiTac;
    }
}
