using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class LichBaoTriQueryRepository : ILichBaoTriQueryRepository
{
    private readonly AppDbContext _dbContext;

    public LichBaoTriQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LichBaoTriDetailResponse?> GetByIdAsync(GetLichBaoTriByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var lichBaoTriMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "l.Id" },
            { "IsDeleted", "l.IsDeleted" }
        };

        var thietBiMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TenThietBi", "t.TenThietBi" },
            { "MaThietBi", "t.MaThietBi" }
        };

        var hangMucMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TenHangMuc", "hm.TenHangMuc" },
            { "MaHangMuc", "hm.MaHangMuc" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, lichBaoTriMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ThietBi", "t", "t.Id = l.ThietBiId", Mapping: thietBiMapping),
            new JoinDefinition("HangMucBaoTri", "hm", "hm.Id = l.HangMucBaoTriId", Mapping: hangMucMapping)
        ], parameters);

        var sql = $"""
            SELECT 
                l.Id, 
                l.ThietBiId, 
                t.TenThietBi, 
                t.MaThietBi, 
                l.HangMucBaoTriId, 
                hm.TenHangMuc, 
                hm.MaHangMuc, 
                l.TanSuatBaoTriId, 
                l.NgayBatDau, 
                l.NgayKetThuc, 
                l.NgayBaoTriGanNhat, 
                l.NgayBaoTriTiepTheo, 
                l.IsActive
            FROM LichBaoTri l
            {sqlJoins}
            {sqlWhere};
            """;

        var result = await connection.QueryFirstOrDefaultAsync<LichBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null)
            return null;

        var freq = TanSuatBaoTri.FromValue(result.TanSuatBaoTriId, null);

        return new LichBaoTriDetailResponse
        {
            Id = result.Id,
            ThietBiId = result.ThietBiId,
            TenThietBi = result.TenThietBi,
            MaThietBi = result.MaThietBi,
            HangMucBaoTriId = result.HangMucBaoTriId,
            TenHangMuc = result.TenHangMuc,
            MaHangMuc = result.MaHangMuc,
            TanSuatBaoTriId = result.TanSuatBaoTriId,
            TenTanSuatBaoTri = freq?.Name ?? string.Empty,
            NgayBatDau = result.NgayBatDau,
            NgayKetThuc = result.NgayKetThuc,
            NgayBaoTriGanNhat = result.NgayBaoTriGanNhat,
            NgayBaoTriTiepTheo = result.NgayBaoTriTiepTheo,
            IsActive = result.IsActive
        };
    }

    public async Task<PagedResult<LichBaoTriResponse>> GetListAsync(GetLichBaoTriListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var lichBaoTriMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "l.Id" },
            { "ThietBiId", "l.ThietBiId" },
            { "HangMucBaoTriId", "l.HangMucBaoTriId" },
            { "IsDeleted", "l.IsDeleted" },
            { "IsActive", "l.IsActive" },
            { "TanSuatBaoTriId", "l.TanSuatBaoTriId" }
        };

        var thietBiMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TenThietBi", "t.TenThietBi" },
            { "MaThietBi", "t.MaThietBi" }
        };

        var hangMucMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TenHangMuc", "hm.TenHangMuc" },
            { "MaHangMuc", "hm.MaHangMuc" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, lichBaoTriMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ThietBi", "t", "t.Id = l.ThietBiId", Mapping: thietBiMapping),
            new JoinDefinition("HangMucBaoTri", "hm", "hm.Id = l.HangMucBaoTriId", Mapping: hangMucMapping)
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, lichBaoTriMapping, "l.Id DESC");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                l.Id, 
                l.ThietBiId, 
                t.TenThietBi, 
                t.MaThietBi, 
                l.HangMucBaoTriId, 
                hm.TenHangMuc, 
                hm.MaHangMuc, 
                l.TanSuatBaoTriId, 
                l.NgayBatDau, 
                l.NgayKetThuc, 
                l.NgayBaoTriGanNhat, 
                l.NgayBaoTriTiepTheo, 
                l.IsActive
            FROM LichBaoTri l
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<LichBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var freqMap = TanSuatBaoTri.ToDictionary();

        var items = rows.Select(r => new LichBaoTriResponse
        {
            Id = r.Id,
            ThietBiId = r.ThietBiId,
            TenThietBi = r.TenThietBi,
            MaThietBi = r.MaThietBi,
            HangMucBaoTriId = r.HangMucBaoTriId,
            TenHangMuc = r.TenHangMuc,
            MaHangMuc = r.MaHangMuc,
            TanSuatBaoTriId = r.TanSuatBaoTriId,
            TenTanSuatBaoTri = freqMap.GetValueOrDefault(r.TanSuatBaoTriId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            NgayBaoTriGanNhat = r.NgayBaoTriGanNhat,
            NgayBaoTriTiepTheo = r.NgayBaoTriTiepTheo,
            IsActive = r.IsActive
        }).ToList();

        return new PagedResult<LichBaoTriResponse>
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
