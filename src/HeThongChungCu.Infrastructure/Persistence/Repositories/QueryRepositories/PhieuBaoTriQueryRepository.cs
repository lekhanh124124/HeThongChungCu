using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriList;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class PhieuBaoTriQueryRepository : IPhieuBaoTriQueryRepository
{
    private readonly AppDbContext _dbContext;

    public PhieuBaoTriQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PhieuBaoTriDetailResponse?> GetByIdAsync(GetPhieuBaoTriByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var phieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "p.Id" },
            { "IsDeleted", "p.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, phieuMapping, parameters);

        var sql = $"""
            SELECT 
                p.Id, 
                p.MaPhieu, 
                p.ThietBiId, 
                t.TenThietBi, 
                t.MaThietBi, 
                p.HangMucBaoTriId, 
                hm.TenHangMuc, 
                p.LichBaoTriId, 
                p.HopDongDoiTacId, 
                hd.SoHopDong,
                dt.TenDoiTac,
                p.NgayLapPhieu, 
                p.NgayDuKien, 
                p.NgayThucTe, 
                p.ChiPhiThucTe, 
                p.TrangThaiPhieuBaoTriId, 
                p.GhiChuXuLy, 
                p.LyDoHuy, 
                p.NguoiKiemDuyetId,
                (nd_kd.Ho + ' ' + nd_kd.Ten) AS TenNguoiKiemDuyet
            FROM PhieuBaoTri p
            INNER JOIN ThietBi t ON t.Id = p.ThietBiId
            INNER JOIN HangMucBaoTri hm ON hm.Id = p.HangMucBaoTriId
            LEFT JOIN HopDongDoiTac hd ON hd.Id = p.HopDongDoiTacId
            LEFT JOIN DoiTac dt ON dt.Id = hd.DoiTacId
            LEFT JOIN NhanVien nv_kd ON nv_kd.Id = p.NguoiKiemDuyetId
            LEFT JOIN NguoiDung nd_kd ON nd_kd.Id = nv_kd.NguoiDungId
            {sqlWhere};
            """;

        var result = await connection.QueryFirstOrDefaultAsync<PhieuBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null)
            return null;

        var status = TrangThaiPhieuBaoTri.FromValue(result.TrangThaiPhieuBaoTriId, null);

        var response = new PhieuBaoTriDetailResponse
        {
            Id = result.Id,
            MaPhieu = result.MaPhieu,
            ThietBiId = result.ThietBiId,
            TenThietBi = result.TenThietBi,
            MaThietBi = result.MaThietBi,
            HangMucBaoTriId = result.HangMucBaoTriId,
            TenHangMuc = result.TenHangMuc,
            LichBaoTriId = result.LichBaoTriId,
            HopDongDoiTacId = result.HopDongDoiTacId,
            SoHopDong = result.SoHopDong,
            TenDoiTac = result.TenDoiTac,
            NgayLapPhieu = result.NgayLapPhieu,
            NgayDuKien = result.NgayDuKien,
            NgayThucTe = result.NgayThucTe,
            ChiPhiThucTe = result.ChiPhiThucTe,
            TrangThaiPhieuBaoTriId = result.TrangThaiPhieuBaoTriId,
            TenTrangThaiPhieuBaoTri = status?.Name ?? string.Empty,
            GhiChuXuLy = result.GhiChuXuLy,
            LyDoHuy = result.LyDoHuy,
            NguoiKiemDuyetId = result.NguoiKiemDuyetId,
            TenNguoiKiemDuyet = result.TenNguoiKiemDuyet
        };

        // Load checklists
        var checklistsSql = "SELECT Id, NoiDungChecklist, DatYeuCau, GhiChuThucTe, AnhMinhHoaId FROM PhieuBaoTriChecklist WHERE PhieuBaoTriId = @PhieuId AND IsDeleted = 0";
        var checklists = await connection.QueryAsync<PhieuBaoTriChecklistDto>(checklistsSql, new { PhieuId = result.Id }, transaction: _dbContext.GetDbTransaction());
        response.Checklists = checklists.ToList();

        // Load materials
        var materialsSql = "SELECT Id, TenVatTu, SoLuong, DonGia, (SoLuong * DonGia) AS ThanhTien FROM PhieuBaoTriVatTu WHERE PhieuBaoTriId = @PhieuId AND IsDeleted = 0";
        var materials = await connection.QueryAsync<PhieuBaoTriVatTuDto>(materialsSql, new { PhieuId = result.Id }, transaction: _dbContext.GetDbTransaction());
        response.VatTus = materials.ToList();

        // Load assigned staff
        var staffSql = """
            SELECT 
                ns.Id,
                ns.NhanVienId,
                COALESCE(nd.Ho + ' ' + nd.Ten, ns.HoTen) AS HoTen,
                COALESCE(nd.Cccd, ns.SoCCCD) AS SoCCCD,
                COALESCE(nd.SoDienThoai, ns.SoDienThoai) AS SoDienThoai,
                ns.VaiTro
            FROM NhanSuBaoTri ns
            LEFT JOIN NhanVien nv ON nv.Id = ns.NhanVienId
            LEFT JOIN NguoiDung nd ON nd.Id = nv.NguoiDungId
            WHERE ns.PhieuBaoTriId = @PhieuId AND ns.IsDeleted = 0
            """;
        var staffs = await connection.QueryAsync<NhanSuBaoTriDto>(staffSql, new { PhieuId = result.Id }, transaction: _dbContext.GetDbTransaction());
        response.NhanSuBaoTris = staffs.ToList();

        return response;
    }

    public async Task<PagedResult<PhieuBaoTriResponse>> GetListAsync(GetPhieuBaoTriListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var phieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "p.Id" },
            { "MaPhieu", "p.MaPhieu" },
            { "ThietBiId", "p.ThietBiId" },
            { "HangMucBaoTriId", "p.HangMucBaoTriId" },
            { "TrangThaiPhieuBaoTriId", "p.TrangThaiPhieuBaoTriId" },
            { "IsDeleted", "p.IsDeleted" },
            { "NgayLapPhieu", "p.NgayLapPhieu" },
            { "NgayDuKien", "p.NgayDuKien" },
            { "NgayThucTe", "p.NgayThucTe" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, phieuMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, phieuMapping, "p.Id DESC");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                p.Id, 
                p.MaPhieu, 
                p.ThietBiId, 
                t.TenThietBi, 
                t.MaThietBi, 
                p.HangMucBaoTriId, 
                hm.TenHangMuc, 
                p.LichBaoTriId, 
                p.HopDongDoiTacId, 
                hd.SoHopDong,
                dt.TenDoiTac,
                p.NgayLapPhieu, 
                p.NgayDuKien, 
                p.NgayThucTe, 
                p.ChiPhiThucTe, 
                p.TrangThaiPhieuBaoTriId, 
                p.GhiChuXuLy, 
                p.LyDoHuy, 
                p.NguoiKiemDuyetId,
                (nd_kd.Ho + ' ' + nd_kd.Ten) AS TenNguoiKiemDuyet
            FROM PhieuBaoTri p
            INNER JOIN ThietBi t ON t.Id = p.ThietBiId
            INNER JOIN HangMucBaoTri hm ON hm.Id = p.HangMucBaoTriId
            LEFT JOIN HopDongDoiTac hd ON hd.Id = p.HopDongDoiTacId
            LEFT JOIN DoiTac dt ON dt.Id = hd.DoiTacId
            LEFT JOIN NhanVien nv_kd ON nv_kd.Id = p.NguoiKiemDuyetId
            LEFT JOIN NguoiDung nd_kd ON nd_kd.Id = nv_kd.NguoiDungId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<PhieuBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var statusMap = TrangThaiPhieuBaoTri.ToDictionary();

        var items = rows.Select(r => new PhieuBaoTriResponse
        {
            Id = r.Id,
            MaPhieu = r.MaPhieu,
            ThietBiId = r.ThietBiId,
            TenThietBi = r.TenThietBi,
            MaThietBi = r.MaThietBi,
            HangMucBaoTriId = r.HangMucBaoTriId,
            TenHangMuc = r.TenHangMuc,
            LichBaoTriId = r.LichBaoTriId,
            HopDongDoiTacId = r.HopDongDoiTacId,
            SoHopDong = r.SoHopDong,
            TenDoiTac = r.TenDoiTac,
            NgayLapPhieu = r.NgayLapPhieu,
            NgayDuKien = r.NgayDuKien,
            NgayThucTe = r.NgayThucTe,
            ChiPhiThucTe = r.ChiPhiThucTe,
            TrangThaiPhieuBaoTriId = r.TrangThaiPhieuBaoTriId,
            TenTrangThaiPhieuBaoTri = statusMap.GetValueOrDefault(r.TrangThaiPhieuBaoTriId, string.Empty),
            GhiChuXuLy = r.GhiChuXuLy,
            LyDoHuy = r.LyDoHuy,
            NguoiKiemDuyetId = r.NguoiKiemDuyetId,
            TenNguoiKiemDuyet = r.TenNguoiKiemDuyet
        }).ToList();

        return new PagedResult<PhieuBaoTriResponse>
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
}
