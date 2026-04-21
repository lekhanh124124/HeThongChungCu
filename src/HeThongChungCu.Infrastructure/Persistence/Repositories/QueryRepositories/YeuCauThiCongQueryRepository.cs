using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauThiCongQueryRepository : IYeuCauThiCongQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauThiCongQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<YeuCauThiCongResponse>> GetAllAsync(
        GetListYeuCauThiCongSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "TrangThaiYeuCauId", "y.TrangThaiId" },
            { "TrangThaiThiCongId", "y.TrangThaiThiCongId" },
            { "CreatedAt", "y.CreatedAt" },
            { "DuKienBatDau", "y.DuKienBatDau" },
            { "DuKienKetThuc", "y.DuKienKetThuc" },
            { "YeuCauIsDeleted", "y.IsDeleted" },
            { "YeuCauLoai", "y.LoaiYeuCauCuDanId" }
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
                y.HangMucThiCong,
                y.DuKienBatDau,
                y.DuKienKetThuc,
                y.TenDonViThiCong,
                y.TrangThaiId AS TrangThaiYeuCauId,
                y.TrangThaiThiCongId,
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
        var rows = (await connection.QueryAsync<YeuCauThiCongReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var trangThaiYeuCauMap = TrangThaiYeuCau.ToDictionary();
        var trangThaiThiCongMap = TrangThaiThiCong.ToDictionary();

        var items = rows.Select(r => new YeuCauThiCongResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            HangMucThiCong = r.HangMucThiCong,
            DuKienBatDau = r.DuKienBatDau,
            DuKienKetThuc = r.DuKienKetThuc,
            TenDonViThiCong = r.TenDonViThiCong,
            TrangThaiYeuCauId = r.TrangThaiYeuCauId,
            TrangThaiYeuCauTen = trangThaiYeuCauMap.GetValueOrDefault(r.TrangThaiYeuCauId, string.Empty),
            TrangThaiThiCongId = r.TrangThaiThiCongId,
            TrangThaiThiCongTen = r.TrangThaiThiCongId.HasValue ? trangThaiThiCongMap.GetValueOrDefault(r.TrangThaiThiCongId.Value, string.Empty) : null,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui
        }).ToList();

        return new PagedResult<YeuCauThiCongResponse>
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

    public async Task<YeuCauThiCongDetailResponse?> GetByIdAsync(
        GetYeuCauThiCongByIdSpecification spec,
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
            new JoinDefinition("NguoiDung", "nd", "y.CreatedBy = nd.Id", JoinType.Left),
            new JoinDefinition("NhanVien", "nv", "y.NguoiXuLyId = nv.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd_xl", "nv.NguoiDungId = nd_xl.Id", JoinType.Left)
        ], parameters);

        // Personnel Sub-query
        var sqlJoinNs = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NhanSuYeuCau", "ns", "ns.YeuCauId = y.Id", JoinType.Inner, Mapping: new() { { "NhanSuIsDeleted", "ns.IsDeleted" }, { "NhanSuLoai", "ns.LoaiNhanSuId" } }),
            new JoinDefinition("NhanVien", "nv_ns", "ns.NhanVienId = nv_ns.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd_ns", "nv_ns.NguoiDungId = nd_ns.Id", JoinType.Left)
        ], parameters);

        // Files Sub-query
        var sqlJoinTtl = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.YeuCauId = y.Id", JoinType.Inner, Mapping: new() { { "TepIsDeleted", "ttl.IsDeleted" }, { "TepLoai", "ttl.LoaiTepId" } })
        ], parameters);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id,
                y.CanHoId,
                ch.TenCanHo,
                y.HangMucThiCong,
                y.DuKienBatDau,
                y.DuKienKetThuc,
                y.TenDonViThiCong,
                y.NguoiDaiDien,
                y.SoDienThoaiDaiDien,
                y.NoiDung,
                y.TrangThaiId AS TrangThaiYeuCauId,
                y.TrangThaiThiCongId,
                y.TienDatCoc,
                y.IsDaThuCoc,
                y.GhiChuThuCoc,
                y.TienKhauTru,
                y.LyDoKhauTru,
                y.IsDaHoanCoc,
                y.CreatedAt,
                y.CreatedBy,
                y.LoaiYeuCauCuDanId,
                nd.Ho + ' ' + nd.Ten AS TenNguoiGui,
                y.NguoiXuLyId,
                nd_xl.Ho + ' ' + nd_xl.Ten AS TenNguoiXuLy,
                y.NgayXuLy,
                y.LyDo
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere};

            -- 2. Personnel
            SELECT
                ns.Id, ns.NhanVienId, ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu, ns.LyDoXoa,
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

        var mainReadModel = await multi.ReadFirstOrDefaultAsync<YeuCauThiCongReadModel>();
        if (mainReadModel == null) return null;

        var personnel = (await multi.ReadAsync<NhanSuThiCongReadModel>()).ToList();
        var files = (await multi.ReadAsync<TepYeuCauThiCongReadModel>()).ToList();

        var trangThaiYeuCauMap = TrangThaiYeuCau.ToDictionary();
        var trangThaiThiCongMap = TrangThaiThiCong.ToDictionary();

        return new YeuCauThiCongDetailResponse
        {
            Id = mainReadModel.Id,
            CanHoId = mainReadModel.CanHoId,
            TenCanHo = mainReadModel.TenCanHo,
            HangMucThiCong = mainReadModel.HangMucThiCong,
            DuKienBatDau = mainReadModel.DuKienBatDau,
            DuKienKetThuc = mainReadModel.DuKienKetThuc,
            TenDonViThiCong = mainReadModel.TenDonViThiCong,
            NguoiDaiDien = mainReadModel.NguoiDaiDien,
            SoDienThoaiDaiDien = mainReadModel.SoDienThoaiDaiDien,
            NoiDung = mainReadModel.NoiDung,
            TrangThaiYeuCauId = mainReadModel.TrangThaiYeuCauId,
            TrangThaiYeuCauTen = trangThaiYeuCauMap.GetValueOrDefault(mainReadModel.TrangThaiYeuCauId, string.Empty),
            TrangThaiThiCongId = mainReadModel.TrangThaiThiCongId,
            TrangThaiThiCongTen = mainReadModel.TrangThaiThiCongId.HasValue ? trangThaiThiCongMap.GetValueOrDefault(mainReadModel.TrangThaiThiCongId.Value, string.Empty) : null,
            TienDatCoc = mainReadModel.TienDatCoc,
            IsDaThuCoc = mainReadModel.IsDaThuCoc,
            GhiChuThuCoc = mainReadModel.GhiChuThuCoc,
            TienKhauTru = mainReadModel.TienKhauTru,
            LyDoKhauTru = mainReadModel.LyDoKhauTru,
            IsDaHoanCoc = mainReadModel.IsDaHoanCoc,
            CreatedAt = mainReadModel.CreatedAt,
            CreatedBy = mainReadModel.CreatedBy,
            TenNguoiGui = mainReadModel.TenNguoiGui,
            NguoiXuLyId = mainReadModel.NguoiXuLyId,
            TenNguoiXuLy = mainReadModel.TenNguoiXuLy,
            NgayXuLy = mainReadModel.NgayXuLy,
            LyDo = mainReadModel.LyDo,
            
            NhanSuThiCongs = personnel.Select(p => new NhanSuThiCongResponse
            {
                Id = p.Id,
                NhanVienId = p.NhanVienId,
                HoTen = p.NhanVienId.HasValue ? $"{p.StaffHo} {p.StaffTen}".Trim() : p.HoTen,
                SoCCCD = p.NhanVienId.HasValue ? p.StaffCCCD ?? string.Empty : p.SoCCCD,
                SoDienThoai = p.NhanVienId.HasValue ? p.StaffPhone : p.SoDienThoai,
                VaiTro = p.VaiTro,
                GhiChu = p.GhiChu,
                LyDoXoa = p.LyDoXoa
            }).ToList(),
            
            DanhSachTep = files.Select(f => new TepYeuCauThiCongResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
        };
    }
}
