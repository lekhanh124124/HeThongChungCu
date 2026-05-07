using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class HoaDonDoiTacQueryRepository : IHoaDonDoiTacQueryRepository
{
    private readonly AppDbContext _dbContext;

    public HoaDonDoiTacQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<HoaDonDoiTacResponse>> GetListAsync(
        GetListHoaDonDoiTacSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "h.Id" },
            { "HopDongDoiTacId", "h.HopDongDoiTacId" },
            { "DoiTacId", "hd.DoiTacId" },
            { "Thang", "h.Thang" },
            { "Nam", "h.Nam" },
            { "TrangThaiThanhToanId", "h.TrangThaiThanhToanId" },
            { "IsDeleted", "h.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "h.NgayGhiNhan");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   h.Id, h.HopDongDoiTacId, hd.SoHopDong, hd.DoiTacId, dt.TenDoiTac,
                   h.Thang, h.Nam, h.SoTien, h.NgayGhiNhan, h.GhiChu, h.TrangThaiThanhToanId,
                   h.FileHoaDonId, tp.FileUrl, tp.FileName, tp.ContentType
            FROM HoaDonDoiTac h
            INNER JOIN HopDongDoiTac hd ON h.HopDongDoiTacId = hd.Id
            INNER JOIN DoiTac dt ON hd.DoiTacId = dt.Id
            LEFT JOIN TepTaiLieu tp ON h.FileHoaDonId = tp.Id
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<HoaDonDoiTacReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var trangThaiThanhToanMap = TrangThaiThanhToanDoiTac.ToDictionary();

        var items = rows.Select(r => new HoaDonDoiTacResponse
        {
            Id = r.Id,
            HopDongDoiTacId = r.HopDongDoiTacId,
            SoHopDong = r.SoHopDong,
            DoiTacId = r.DoiTacId,
            TenDoiTac = r.TenDoiTac,
            Thang = r.Thang,
            Nam = r.Nam,
            SoTien = r.SoTien,
            NgayGhiNhan = r.NgayGhiNhan,
            GhiChu = r.GhiChu,
            TrangThaiThanhToanId = r.TrangThaiThanhToanId,
            TrangThaiThanhToanTen = trangThaiThanhToanMap.GetValueOrDefault(r.TrangThaiThanhToanId, string.Empty),
            FileHoaDonId = r.FileHoaDonId,
            FileHoaDonUrl = r.FileUrl,
            FileHoaDonName = r.FileName
        }).ToList();

        return new PagedResult<HoaDonDoiTacResponse>
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

    public async Task<HoaDonDoiTacDetailResponse?> GetByIdAsync(
        GetHoaDonDoiTacByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "h.Id" },
            { "IsDeleted", "h.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sql = $"""
            SELECT h.Id, h.HopDongDoiTacId, hd.SoHopDong, hd.NoiDung AS NoiDungHopDong,
                   hd.DoiTacId, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien AS NguoiDaiDienDoiTac, dt.SoDienThoai AS SoDienThoaiDoiTac, dt.Email AS EmailDoiTac,
                   h.Thang, h.Nam, h.SoTien, h.NgayGhiNhan, h.GhiChu, h.TrangThaiThanhToanId,
                   h.FileHoaDonId, tp.FileUrl, tp.FileName, tp.ContentType,
                   h.CreatedAt, h.CreatedBy, nd_c.Ho + ' ' + nd_c.Ten AS TenNguoiTao,
                   h.ModifiedAt, h.ModifiedBy, nd_m.Ho + ' ' + nd_m.Ten AS TenNguoiSua
            FROM HoaDonDoiTac h
            INNER JOIN HopDongDoiTac hd ON h.HopDongDoiTacId = hd.Id
            INNER JOIN DoiTac dt ON hd.DoiTacId = dt.Id
            LEFT JOIN TepTaiLieu tp ON h.FileHoaDonId = tp.Id
            LEFT JOIN NguoiDung nd_c ON h.CreatedBy = nd_c.Id
            LEFT JOIN NguoiDung nd_m ON h.ModifiedBy = nd_m.Id
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var r = await connection.QueryFirstOrDefaultAsync<HoaDonDoiTacDetailReadModel>(sql, parameters, transaction: transaction);

        if (r == null) return null;

        var trangThaiThanhToanMap = TrangThaiThanhToanDoiTac.ToDictionary();

        return new HoaDonDoiTacDetailResponse
        {
            Id = r.Id,
            HopDongDoiTacId = r.HopDongDoiTacId,
            SoHopDong = r.SoHopDong,
            DoiTacId = r.DoiTacId,
            TenDoiTac = r.TenDoiTac,
            Thang = r.Thang,
            Nam = r.Nam,
            SoTien = r.SoTien,
            NgayGhiNhan = r.NgayGhiNhan,
            GhiChu = r.GhiChu,
            TrangThaiThanhToanId = r.TrangThaiThanhToanId,
            TrangThaiThanhToanTen = trangThaiThanhToanMap.GetValueOrDefault(r.TrangThaiThanhToanId, string.Empty),
            FileHoaDonId = r.FileHoaDonId,
            FileHoaDonUrl = r.FileUrl,
            FileHoaDonName = r.FileName,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.TenNguoiTao, // Show creator name
            UpdatedAt = r.ModifiedAt,
            UpdatedBy = r.TenNguoiSua, // Show updater name
            TenCongTy = r.TenCongTy,
            NguoiDaiDienDoiTac = r.NguoiDaiDienDoiTac,
            SoDienThoaiDoiTac = r.SoDienThoaiDoiTac,
            EmailDoiTac = r.EmailDoiTac,
            NoiDungHopDong = r.NoiDungHopDong
        };
    }
}
