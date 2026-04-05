using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class NhanVienQueryRepository : INhanVienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public NhanVienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NhanVienResponse?> GetByIdAsync(GetNhanVienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "IsDeleted", "nv.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var joins = new[]
        {
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Type: JoinType.Inner),
            new JoinDefinition("TaiKhoan", "a", "u.Id = a.NguoiDungId AND a.IsActive = 1"),
            new JoinDefinition("TepTaiLieu", "atl", "a.AnhDaiDienId = atl.Id"),
            new JoinDefinition("PhanQuyen", "pq", "a.Id = pq.TaiKhoanId", AddSoftDelete: false),
            new JoinDefinition("TaiLieu", "t", "t.NguoiDungId = u.Id", Discriminator: ("LoaiTaiLieu", "TaiLieuNguoiDung")),
            new JoinDefinition("TepTaiLieu", "f", "f.TaiLieuId = t.Id", Discriminator: ("LoaiTepTaiLieu", "TepTaiLieuNguoiDung"))
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT 
                nv.Id,
                nv.NguoiDungId,
                u.Ten AS FirstName,
                u.Ho AS LastName,
                u.Ho + ' ' + u.Ten AS HoTen,
                a.Email,
                u.SoDienThoai,
                u.CCCD,
                u.DiaChi,
                u.NgaySinh AS Dob,
                u.GioiTinhId,
                atl.FileUrl AS AnhDaiDienUrl,
                pq.RoleId,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu,
                t.Id AS DocId, t.LoaiGiayToId, t.SoGiayTo, t.NgayPhatHanh,
                f.Id AS FileId, f.FileUrl, f.FileName, f.ContentType
            FROM NhanVien nv
            {sqlJoins}
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<dynamic>(sql, parameters);
        
        NhanVienResponse? response = null;
        var docLookup = new Dictionary<int, TaiLieuResponse>();
        var roleIds = new HashSet<int>();

        var gioiTinhMap = GioiTinh.ToDictionary();
        var roleMap = Role.ToDictionary();

        foreach (var row in rows)
        {
            if (response == null)
            {
                response = new NhanVienResponse
                {
                    Id = row.Id,
                    NguoiDungId = row.NguoiDungId,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    HoTen = row.HoTen,
                    Email = row.Email ?? string.Empty,
                    SoDienThoai = row.SoDienThoai,
                    CCCD = row.CCCD,
                    DiaChi = row.DiaChi,
                    Dob = row.Dob,
                    GioiTinhId = row.GioiTinhId,
                    GioiTinhName = gioiTinhMap.GetValueOrDefault((int)row.GioiTinhId, string.Empty),
                    AnhDaiDienUrl = row.AnhDaiDienUrl ?? string.Empty,
                    LoaiNhanVienId = row.LoaiNhanVienId,
                    TenLoaiNhanVien = LoaiNhanVien.FromValue((int)row.LoaiNhanVienId)?.Name ?? string.Empty,
                    TrangThaiNhanVienId = row.TrangThaiNhanVienId,
                    TenTrangThaiNhanVien = TrangThaiNhanVien.FromValue((int)row.TrangThaiNhanVienId)?.Name ?? string.Empty,
                    MaNhanVien = row.MaNhanVien,
                    NgayVaoLam = row.NgayVaoLam,
                    NgayNghiLam = row.NgayNghiLam,
                    GhiChu = row.GhiChu,
                    Roles = [],
                    TaiLieuNguoiDungs = []
                };
            }

            if (row.RoleId != null)
            {
                int rId = (int)row.RoleId;
                if (!roleIds.Contains(rId))
                {
                    roleIds.Add(rId);
                    response.Roles.Add(roleMap.GetValueOrDefault(rId, string.Empty));
                }
            }

            if (row.DocId != null)
            {
                if (!docLookup.TryGetValue((int)row.DocId, out var doc))
                {
                    doc = new TaiLieuResponse
                    {
                        Id = row.DocId,
                        LoaiGiayToId = (int)row.LoaiGiayToId,
                        TenLoaiGiayTo = LoaiGiayTo.FromValue((int)row.LoaiGiayToId)?.Name ?? string.Empty,
                        SoGiayTo = row.SoGiayTo,
                        NgayPhatHanh = row.NgayPhatHanh,
                        Files = []
                    };
                    docLookup.Add(doc.Id, doc);
                    response.TaiLieuNguoiDungs.Add(doc);
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

        return response;
    }

    public async Task<PagedResult<NhanVienResponse>> GetListAsync(GetNhanVienListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "MaNhanVien", "nv.MaNhanVien" },
            { "LoaiNhanVienId", "nv.LoaiNhanVienId" },
            { "TrangThaiNhanVienId", "nv.TrangThaiNhanVienId" },
            { "IsDeleted", "nv.IsDeleted" },
            { "Keyword", "nv.MaNhanVien + ' ' + u.Ho + ' ' + u.Ten + ' ' + u.SoDienThoai" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var joins = new[]
        {
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Type: JoinType.Inner)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                nv.Id,
                nv.NguoiDungId,
                u.Ho + ' ' + u.Ten AS HoTen,
                u.SoDienThoai,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu
            FROM NhanVien nv
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<NhanVienReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(MapToResponse).ToList();

        return new PagedResult<NhanVienResponse>
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

    private static NhanVienResponse MapToResponse(NhanVienReadModel model)
    {
        return new NhanVienResponse
        {
            Id = model.Id,
            NguoiDungId = model.NguoiDungId,
            HoTen = model.HoTen,
            SoDienThoai = model.SoDienThoai,
            LoaiNhanVienId = model.LoaiNhanVienId,
            TenLoaiNhanVien = LoaiNhanVien.FromValue(model.LoaiNhanVienId)?.Name ?? string.Empty,
            TrangThaiNhanVienId = model.TrangThaiNhanVienId,
            TenTrangThaiNhanVien = TrangThaiNhanVien.FromValue(model.TrangThaiNhanVienId)?.Name ?? string.Empty,
            MaNhanVien = model.MaNhanVien,
            NgayVaoLam = model.NgayVaoLam,
            NgayNghiLam = model.NgayNghiLam,
            GhiChu = model.GhiChu
        };
    }
}
