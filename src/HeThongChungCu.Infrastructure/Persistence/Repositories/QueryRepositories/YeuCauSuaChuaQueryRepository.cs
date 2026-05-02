using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauSuaChuaQueryRepository : IYeuCauSuaChuaQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauSuaChuaQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<YeuCauSuaChuaResponse>> GetAllAsync(
        GetListYeuCauSuaChuaSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "TrangThaiSuaChuaId", "y.TrangThaiSuaChuaId" },
            { "TrangThaiYeuCauId", "y.TrangThaiId" },
            { "LoaiSuCoId", "y.LoaiSuCoId" },
            { "CreatedAt", "y.CreatedAt" },
            { "YeuCauIsDeleted", "y.IsDeleted" },
            { "YeuCauLoai", "y.LoaiYeuCauCuDanId" },
            { "MaCanHo", "ch.MaCanHo" },
            { "TenNguoiGui", "nd.Ho + ' ' + nd.Ten" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id", JoinType.Inner, Mapping: new() { { "CanHoIsDeleted", "ch.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd", "y.CreatedBy = nd.Id", JoinType.Left)
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                ch.TenCanHo,
                y.NoiDung,
                y.TrangThaiSuaChuaId,
                y.LoaiSuCoId,
                y.TrangThaiId AS TrangThaiYeuCauId,
                y.CreatedAt,
                y.CreatedBy,
                y.LoaiYeuCauCuDanId,
                nd.Ho + ' ' + nd.Ten AS TenNguoiGui
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<YeuCauSuaChuaReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var trangThaiSuaChuaMap = TrangThaiSuaChua.ToDictionary();
        var loaiSuCoMap = LoaiSuCoKyThuat.ToDictionary();
        var trangThaiYeuCauMap = TrangThaiYeuCau.ToDictionary();
        var loaiYeuCauMap = LoaiYeuCauCuDan.ToDictionary();

        var items = rows.Select(r => new YeuCauSuaChuaResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            NoiDung = r.NoiDung,
            TrangThaiSuaChuaId = r.TrangThaiSuaChuaId,
            TrangThaiSuaChuaTen = r.TrangThaiSuaChuaId.HasValue ? trangThaiSuaChuaMap.GetValueOrDefault(r.TrangThaiSuaChuaId.Value, string.Empty) : null,
            LoaiSuCoId = r.LoaiSuCoId,
            LoaiSuCoTen = loaiSuCoMap.GetValueOrDefault(r.LoaiSuCoId, string.Empty),
            TrangThaiYeuCauId = r.TrangThaiYeuCauId,
            TrangThaiYeuCauTen = trangThaiYeuCauMap.GetValueOrDefault(r.TrangThaiYeuCauId, string.Empty),
            LoaiYeuCauCuDanId = r.LoaiYeuCauCuDanId,
            LoaiYeuCauCuDanTen = loaiYeuCauMap.GetValueOrDefault(r.LoaiYeuCauCuDanId, string.Empty),
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui
        }).ToList();

        return new PagedResult<YeuCauSuaChuaResponse>
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

    public async Task<YeuCauSuaChuaDetailResponse?> GetByIdAsync(
        GetYeuCauSuaChuaByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "YeuCauIsDeleted", "y.IsDeleted" },
            { "YeuCauLoai", "y.LoaiYeuCauCuDanId" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id", JoinType.Inner, Mapping: new() { { "CanHoIsDeleted", "ch.IsDeleted" } }),
            new JoinDefinition("HopDongDoiTac", "hd", "y.HopDongDoiTacId = hd.Id", JoinType.Left),
            new JoinDefinition("DoiTac", "dt", "hd.DoiTacId = dt.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd", "y.CreatedBy = nd.Id", JoinType.Left),
            new JoinDefinition("NhanVien", "nv", "y.NguoiXuLyId = nv.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd_xl", "nv.NguoiDungId = nd_xl.Id", JoinType.Left)
        ], parameters);

        // Sub-query for Personnel
        var sqlJoinNs = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NhanSuYeuCau", "ns", "ns.YeuCauId = y.Id", JoinType.Inner, Mapping: new() { { "NhanSuIsDeleted", "ns.IsDeleted" }, { "NhanSuLoai", "ns.LoaiNhanSuId" } }),
            new JoinDefinition("NhanVien", "nv_ns", "ns.NhanVienId = nv_ns.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd_ns", "nv_ns.NguoiDungId = nd_ns.Id", JoinType.Left)
        ], parameters);

        // Sub-query for Files
        var sqlJoinTtl = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.YeuCauId = y.Id", JoinType.Inner, Mapping: new() { { "TepIsDeleted", "ttl.IsDeleted" }, { "TepLoai", "ttl.LoaiTepId" } })
        ], parameters);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id,
                y.CanHoId,
                ch.TenCanHo,
                y.NoiDung,
                y.TrangThaiSuaChuaId,
                y.LoaiSuCoId,
                y.TrangThaiId AS TrangThaiYeuCauId,
                y.CreatedAt,
                y.CreatedBy,
                y.LoaiYeuCauCuDanId,
                nd.Ho + ' ' + nd.Ten AS TenNguoiGui,
                y.PhamViId,
                y.MoTaViTri,
                y.HenTu,
                y.HenDen,
                y.KetQuaXuLy,
                y.LyDoHuy,
                y.ChiPhiDuKien,
                y.ChiPhiThucTe,
                y.IsMienPhi,
                y.GhiChuBaoGia,
                y.HopDongDoiTacId,
                dt.TenDoiTac,
                y.NguoiXuLyId,
                nd_xl.Ho + ' ' + nd_xl.Ten AS TenNguoiXuLy,
                y.NgayXuLy,
                y.LyDo
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere};

            -- 2. Personnel
            SELECT
                ns.Id, ns.NhanVienId, ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu,
                nd_ns.Ho AS StaffHo, nd_ns.Ten AS StaffTen, nd_ns.CCCD AS StaffCCCD, nd_ns.SoDienThoai AS StaffPhone
            FROM YeuCau y
            {sqlJoinNs}
            {sqlWhere};

            -- 3. Files
            SELECT
                ttl.Id, ttl.FileName, ttl.FileUrl, ttl.ContentType
            FROM YeuCau y
            {sqlJoinTtl}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var mainReadModel = await multi.ReadFirstOrDefaultAsync<YeuCauSuaChuaReadModel>();
        if (mainReadModel == null) return null;

        var personnel = (await multi.ReadAsync<NhanSuSuaChuaReadModel>()).ToList();
        var files = (await multi.ReadAsync<TepYeuCauSuaChuaReadModel>()).ToList();

        var trangThaiSuaChuaMap = TrangThaiSuaChua.ToDictionary();
        var loaiSuCoMap = LoaiSuCoKyThuat.ToDictionary();
        var trangThaiYeuCauMap = TrangThaiYeuCau.ToDictionary();
        var phamViMap = PhamViSuaChua.ToDictionary();
        var loaiYeuCauMap = LoaiYeuCauCuDan.ToDictionary();

        return new YeuCauSuaChuaDetailResponse
        {
            Id = mainReadModel.Id,
            CanHoId = mainReadModel.CanHoId,
            TenCanHo = mainReadModel.TenCanHo,
            NoiDung = mainReadModel.NoiDung,
            TrangThaiSuaChuaId = mainReadModel.TrangThaiSuaChuaId,
            TrangThaiSuaChuaTen = mainReadModel.TrangThaiSuaChuaId.HasValue ? trangThaiSuaChuaMap.GetValueOrDefault(mainReadModel.TrangThaiSuaChuaId.Value, string.Empty) : null,
            LoaiSuCoId = mainReadModel.LoaiSuCoId,
            LoaiSuCoTen = loaiSuCoMap.GetValueOrDefault(mainReadModel.LoaiSuCoId, string.Empty),
            TrangThaiYeuCauId = mainReadModel.TrangThaiYeuCauId,
            TrangThaiYeuCauTen = trangThaiYeuCauMap.GetValueOrDefault(mainReadModel.TrangThaiYeuCauId, string.Empty),
            LoaiYeuCauCuDanId = mainReadModel.LoaiYeuCauCuDanId,
            LoaiYeuCauCuDanTen = loaiYeuCauMap.GetValueOrDefault(mainReadModel.LoaiYeuCauCuDanId, string.Empty),
            CreatedAt = mainReadModel.CreatedAt,
            CreatedBy = mainReadModel.CreatedBy,
            TenNguoiGui = mainReadModel.TenNguoiGui,

            LyDo = mainReadModel.LyDo,
            NguoiXuLyId = mainReadModel.NguoiXuLyId,
            TenNguoiXuLy = mainReadModel.TenNguoiXuLy,
            NgayXuLy = mainReadModel.NgayXuLy,
            PhamViId = mainReadModel.PhamViId,
            PhamViTen = mainReadModel.PhamViId.HasValue ? phamViMap.GetValueOrDefault(mainReadModel.PhamViId.Value, string.Empty) : null,
            MoTaViTri = mainReadModel.MoTaViTri,
            HenTu = mainReadModel.HenTu,
            HenDen = mainReadModel.HenDen,
            ChiPhiDuKien = mainReadModel.ChiPhiDuKien,
            ChiPhiThucTe = mainReadModel.ChiPhiThucTe,
            IsMienPhi = mainReadModel.IsMienPhi,
            GhiChuBaoGia = mainReadModel.GhiChuBaoGia,
            KetQuaXuLy = mainReadModel.KetQuaXuLy,
            LyDoHuy = mainReadModel.LyDoHuy,
            HopDongDoiTacId = mainReadModel.HopDongDoiTacId,
            TenDoiTac = mainReadModel.TenDoiTac,
            NhanSuSuaChuas = personnel.Select(p => new NhanSuSuaChuaResponse
            {
                Id = p.Id,
                NhanVienId = p.NhanVienId,
                HoTen = p.NhanVienId.HasValue ? $"{p.StaffHo} {p.StaffTen}".Trim() : p.HoTen,
                SoCCCD = p.NhanVienId.HasValue ? p.StaffCCCD ?? string.Empty : p.SoCCCD,
                SoDienThoai = p.NhanVienId.HasValue ? p.StaffPhone : p.SoDienThoai,
                VaiTro = p.VaiTro,
                GhiChu = p.GhiChu
            }).ToList(),
            DanhSachTep = files.Select(f => new TepTaiLieuResponse(
                f.Id,
                f.FileUrl,
                f.FileName,
                f.ContentType
            )).ToList()
        };
    }
}
