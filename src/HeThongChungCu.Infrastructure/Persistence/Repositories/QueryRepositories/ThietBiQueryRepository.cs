using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class ThietBiQueryRepository : IThietBiQueryRepository
{
    private readonly AppDbContext _dbContext;

    public ThietBiQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThietBiDetailResponse?> GetByIdAsync(GetThietBiByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var thietBiMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, thietBiMapping, parameters);

        var sql = $"""
            SELECT 
                t.Id, 
                t.MaThietBi, 
                t.TenThietBi, 
                t.LoaiThietBi, 
                t.ViTri, 
                t.NgayMua, 
                t.NgayHetHanBaoHanh, 
                t.GiaTriBanDau, 
                t.TrangThaiThietBiId, 
                t.GhiChu
            FROM ThietBi t
            {sqlWhere};
            """;

        var result = await connection.QueryFirstOrDefaultAsync<ThietBiReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null)
            return null;

        var status = TrangThaiThietBi.FromValue(result.TrangThaiThietBiId, null);
 
        return new ThietBiDetailResponse
        {
            Id = result.Id,
            MaThietBi = result.MaThietBi,
            TenThietBi = result.TenThietBi,
            LoaiThietBi = result.LoaiThietBi,
            ViTri = result.ViTri,
            NgayMua = result.NgayMua,
            NgayHetHanBaoHanh = result.NgayHetHanBaoHanh,
            GiaTriBanDau = result.GiaTriBanDau,
            TrangThaiThietBiId = result.TrangThaiThietBiId,
            TenTrangThaiThietBi = status?.Name ?? string.Empty,
            GhiChu = result.GhiChu
        };
    }

    public async Task<PagedResult<ThietBiResponse>> GetListAsync(GetThietBiListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var thietBiMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "MaThietBi", "t.MaThietBi" },
            { "TenThietBi", "t.TenThietBi" },
            { "LoaiThietBi", "t.LoaiThietBi" },
            { "TrangThaiThietBiId", "t.TrangThaiThietBiId" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, thietBiMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, thietBiMapping, "t.Id DESC");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                t.Id, 
                t.MaThietBi, 
                t.TenThietBi, 
                t.LoaiThietBi, 
                t.ViTri, 
                t.NgayMua, 
                t.NgayHetHanBaoHanh, 
                t.GiaTriBanDau, 
                t.TrangThaiThietBiId, 
                t.GhiChu
            FROM ThietBi t
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<ThietBiReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var statusMap = TrangThaiThietBi.ToDictionary();

        var items = rows.Select(r => new ThietBiResponse
        {
            Id = r.Id,
            MaThietBi = r.MaThietBi,
            TenThietBi = r.TenThietBi,
            LoaiThietBi = r.LoaiThietBi,
            ViTri = r.ViTri,
            NgayMua = r.NgayMua,
            NgayHetHanBaoHanh = r.NgayHetHanBaoHanh,
            GiaTriBanDau = r.GiaTriBanDau,
            TrangThaiThietBiId = r.TrangThaiThietBiId,
            TenTrangThaiThietBi = statusMap.GetValueOrDefault(r.TrangThaiThietBiId, string.Empty),
            GhiChu = r.GhiChu
        }).ToList();

        return new PagedResult<ThietBiResponse>
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
