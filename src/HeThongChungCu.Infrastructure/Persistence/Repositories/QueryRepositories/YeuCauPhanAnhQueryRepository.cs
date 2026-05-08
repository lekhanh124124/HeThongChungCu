using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauPhanAnhQueryRepository : IYeuCauPhanAnhQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauPhanAnhQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<PhanAnhResponse>> GetAllAsync(
        GetPhanAnhListSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "LoaiPhanAnhId", "y.LoaiPhanAnhId" },
            { "TrangThaiPhanAnhId", "y.TrangThaiPhanAnhId" },
            { "NguoiXuLyId", "y.NguoiXuLyId" },
            { "TieuDe", "y.TieuDe" },
            { "NoiDung", "y.NoiDung" },
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
            new JoinDefinition("NguoiDung", "nd", "y.CreatedBy = nd.Id", JoinType.Left),
            new JoinDefinition("NhanVien", "nv", "y.NguoiXuLyId = nv.Id", JoinType.Left),
            new JoinDefinition("NguoiDung", "nd_xl", "nv.NguoiDungId = nd_xl.Id", JoinType.Left)
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                ch.TenCanHo,
                y.TieuDe,
                y.LoaiPhanAnhId,
                y.TrangThaiPhanAnhId,
                y.NguoiXuLyId,
                nd_xl.Ho + ' ' + nd_xl.Ten AS TenNguoiXuLy,
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
        var rows = (await connection.QueryAsync<YeuCauPhanAnhReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var trangThaiMap = TrangThaiPhanAnh.ToDictionary();
        var loaiMap = LoaiPhanAnh.ToDictionary();

        var items = rows.Select(r => new PhanAnhResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            TieuDe = r.TieuDe,
            LoaiPhanAnhId = r.LoaiPhanAnhId,
            LoaiPhanAnhTen = loaiMap.GetValueOrDefault(r.LoaiPhanAnhId, string.Empty),
            TrangThaiPhanAnhId = r.TrangThaiPhanAnhId,
            TrangThaiPhanAnhTen = trangThaiMap.GetValueOrDefault(r.TrangThaiPhanAnhId, string.Empty),
            NguoiXuLyId = r.NguoiXuLyId,
            TenNguoiXuLy = string.IsNullOrWhiteSpace(r.TenNguoiXuLy) ? null : r.TenNguoiXuLy,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui
        }).ToList();

        return new PagedResult<PhanAnhResponse>
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

    public async Task<PhanAnhDetailResponse?> GetByIdAsync(
        GetPhanAnhByIdSpecification spec,
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

        var sqlJoinTtl = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.YeuCauId = y.Id", JoinType.Inner, Mapping: new() { { "TepIsDeleted", "ttl.IsDeleted" }, { "TepLoai", "ttl.LoaiTepId" } })
        ], parameters);

        var sqlJoinTraLoi = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TraLoiPhanAnh", "tl", "tl.YeuCauPhanAnhId = y.Id", JoinType.Inner, Mapping: new() { { "TraLoiIsDeleted", "tl.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd_tl", "tl.CreatedBy = nd_tl.Id", JoinType.Left)
        ], parameters);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id,
                y.CanHoId,
                ch.TenCanHo,
                y.TieuDe,
                y.NoiDung,
                y.LoaiPhanAnhId,
                y.TrangThaiPhanAnhId,
                y.NguoiXuLyId,
                nd_xl.Ho + ' ' + nd_xl.Ten AS TenNguoiXuLy,
                y.CreatedAt,
                y.CreatedBy,
                nd.Ho + ' ' + nd.Ten AS TenNguoiGui,
                y.DiemDanhGia,
                y.NhanXetDanhGia,
                y.NgayDanhGia
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere};

            -- 2. Chat Replies
            SELECT
                tl.Id,
                tl.NoiDung,
                tl.IsNhanVien,
                tl.CreatedBy,
                nd_tl.Ho + ' ' + nd_tl.Ten AS TenNguoiGui,
                tl.CreatedAt
            FROM YeuCau y
            {sqlJoinTraLoi}
            {sqlWhere}
            ORDER BY tl.CreatedAt ASC;

            -- 3. Files
            SELECT
                ttl.Id,
                ttl.FileName,
                ttl.FileUrl,
                ttl.ContentType
            FROM YeuCau y
            {sqlJoinTtl}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var mainReadModel = await multi.ReadFirstOrDefaultAsync<YeuCauPhanAnhReadModel>();
        if (mainReadModel == null) return null;

        var replies = (await multi.ReadAsync<TraLoiPhanAnhReadModel>()).ToList();
        var files = (await multi.ReadAsync<TepYeuCauPhanAnhReadModel>()).ToList();

        var trangThaiMap = TrangThaiPhanAnh.ToDictionary();
        var loaiMap = LoaiPhanAnh.ToDictionary();

        return new PhanAnhDetailResponse
        {
            Id = mainReadModel.Id,
            CanHoId = mainReadModel.CanHoId,
            TenCanHo = mainReadModel.TenCanHo,
            TieuDe = mainReadModel.TieuDe,
            NoiDung = mainReadModel.NoiDung,
            LoaiPhanAnhId = mainReadModel.LoaiPhanAnhId,
            LoaiPhanAnhTen = loaiMap.GetValueOrDefault(mainReadModel.LoaiPhanAnhId, string.Empty),
            TrangThaiPhanAnhId = mainReadModel.TrangThaiPhanAnhId,
            TrangThaiPhanAnhTen = trangThaiMap.GetValueOrDefault(mainReadModel.TrangThaiPhanAnhId, string.Empty),
            NguoiXuLyId = mainReadModel.NguoiXuLyId,
            TenNguoiXuLy = string.IsNullOrWhiteSpace(mainReadModel.TenNguoiXuLy) ? null : mainReadModel.TenNguoiXuLy,
            CreatedAt = mainReadModel.CreatedAt,
            CreatedBy = mainReadModel.CreatedBy,
            TenNguoiGui = mainReadModel.TenNguoiGui,
            DiemDanhGia = mainReadModel.DiemDanhGia,
            NhanXetDanhGia = mainReadModel.NhanXetDanhGia,
            NgayDanhGia = mainReadModel.NgayDanhGia,

            TraLoiPhanAnhs = replies.Select(r => new TraLoiPhanAnhResponse
            {
                Id = r.Id,
                NoiDung = r.NoiDung,
                IsNhanVien = r.IsNhanVien,
                CreatedBy = r.CreatedBy,
                TenNguoiGui = r.TenNguoiGui,
                CreatedAt = r.CreatedAt
            }).ToList(),

            DanhSachTep = files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
        };
    }
}
