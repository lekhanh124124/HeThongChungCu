using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class DotThanhToanQueryRepository : IDotThanhToanQueryRepository
{
    private readonly AppDbContext _dbContext;

    public DotThanhToanQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DotThanhToanResponse>> GetListAsync(
        GetListDotThanhToanSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dt.Id" },
            { "TenDot", "dt.TenDot" },
            { "Thang", "dt.Thang" },
            { "Nam", "dt.Nam" },
            { "TrangThaiDotThanhToanId", "dt.TrangThaiDotThanhToanId" },
            { "NgayPhatHanh", "dt.NgayPhatHanh" },
            { "IsDeleted", "dt.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   dt.Id, dt.TenDot, dt.Thang, dt.Nam, 
                   dt.TrangThaiDotThanhToanId, dt.NgayPhatHanh, dt.GhiChu
            FROM DotThanhToan dt
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<DotThanhToanReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DotThanhToanResponse
        {
            Id = r.Id,
            TenDot = r.TenDot,
            Thang = r.Thang,
            Nam = r.Nam,
            TrangThaiDotThanhToanId = r.TrangThaiDotThanhToanId,
            TrangThaiDotThanhToanTen = TrangThaiDotThanhToan.FromValue(r.TrangThaiDotThanhToanId)?.Name ?? string.Empty,
            NgayPhatHanh = r.NgayPhatHanh,
            GhiChu = r.GhiChu
        }).ToList();

        return new PagedResult<DotThanhToanResponse>
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

    public async Task<DotThanhToanDetailResponse?> GetByIdAsync(
        GetDotThanhToanByIdSpecification spec,
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

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sql = $"""
            SELECT dt.Id, dt.TenDot, dt.Thang, dt.Nam, 
                   dt.TrangThaiDotThanhToanId, dt.NgayPhatHanh, dt.GhiChu
            FROM DotThanhToan dt
            {sqlWhere};
            """;

        var row = await connection.QueryFirstOrDefaultAsync<DotThanhToanReadModel>(sql, parameters);
        if (row == null) return null;

        return new DotThanhToanDetailResponse
        {
            Id = row.Id,
            TenDot = row.TenDot,
            Thang = row.Thang,
            Nam = row.Nam,
            TrangThaiDotThanhToanId = row.TrangThaiDotThanhToanId,
            TrangThaiDotThanhToanTen = TrangThaiDotThanhToan.FromValue(row.TrangThaiDotThanhToanId)?.Name ?? string.Empty,
            NgayPhatHanh = row.NgayPhatHanh,
            GhiChu = row.GhiChu
        };
    }
}
