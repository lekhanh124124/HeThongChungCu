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

    public async Task<PagedResult<YeuCauCuTruResponse>> GetPagedListAsync(
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
            { "CreatedAt", "y.CreatedAt" }
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
                y.YeuCauTen,
                y.YeuCauHo,
                y.YeuCauNgaySinh,
                y.YeuCauGioiTinhId,
                y.YeuCauSoDienThoai,
                y.YeuCauCCCD,
                y.YeuCauDiaChi,
                y.YeuCauLoaiQuanHeId,
                y.QuanHeCuTruId
            FROM YeuCauCuTru y
            {(string.IsNullOrEmpty(sqlWhere) ? "" : sqlWhere)}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<YeuCauCuTruReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        var items = rows.Select(r => new YeuCauCuTruResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            LoaiYeuCauId = r.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(r.LoaiYeuCauId, string.Empty),
            TrangThaiId = r.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(r.TrangThaiId, string.Empty),
            LyDo = r.LyDo,
            NoiDung = r.NoiDung,
            CreatedAt = r.CreatedAt,
            NgayXuLy = r.NgayXuLy,
            NguoiXuLyId = r.NguoiXuLyId,
            YeuCauTen = r.YeuCauTen,
            YeuCauHo = r.YeuCauHo,
            YeuCauNgaySinh = r.YeuCauNgaySinh,
            YeuCauGioiTinhId = r.YeuCauGioiTinhId,
            YeuCauSoDienThoai = r.YeuCauSoDienThoai,
            YeuCauCCCD = r.YeuCauCCCD,
            YeuCauDiaChi = r.YeuCauDiaChi,
            YeuCauLoaiQuanHeId = r.YeuCauLoaiQuanHeId,
            TargetQuanHeCuTruId = r.QuanHeCuTruId
        }).ToList();

        return new PagedResult<YeuCauCuTruResponse>
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
