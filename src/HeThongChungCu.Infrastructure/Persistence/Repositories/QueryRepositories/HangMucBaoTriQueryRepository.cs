using System.Data;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class HangMucBaoTriQueryRepository : IHangMucBaoTriQueryRepository
{
    private readonly AppDbContext _dbContext;

    public HangMucBaoTriQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HangMucBaoTriDetailResponse?> GetByIdAsync(GetHangMucBaoTriByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hm.Id" },
            { "IsDeleted", "hm.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, mapping, parameters);

        var sql = $"""
            SELECT 
                hm.Id, 
                hm.MaHangMuc, 
                hm.TenHangMuc, 
                hm.MoTa, 
                hm.ThoiGianUocTinhPhut, 
                hm.ChiPhiUocTinh, 
                hm.ChecklistTieuChuan
            FROM HangMucBaoTri hm
            {sqlWhere};
            """;

        var result = await connection.QueryFirstOrDefaultAsync<HangMucBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null)
            return null;

        List<string> checklist = new();
        if (!string.IsNullOrWhiteSpace(result.ChecklistTieuChuan))
        {
            try
            {
                checklist = System.Text.Json.JsonSerializer.Deserialize<List<string>>(result.ChecklistTieuChuan) ?? new();
            }
            catch
            {
                // Ignore parse errors, return empty list
            }
        }

        return new HangMucBaoTriDetailResponse
        {
            Id = result.Id,
            MaHangMuc = result.MaHangMuc,
            TenHangMuc = result.TenHangMuc,
            MoTa = result.MoTa,
            ThoiGianUocTinhPhut = result.ThoiGianUocTinhPhut,
            ChiPhiUocTinh = result.ChiPhiUocTinh,
            ChecklistTieuChuan = checklist
        };
    }

    public async Task<PagedResult<HangMucBaoTriResponse>> GetListAsync(GetHangMucBaoTriListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hm.Id" },
            { "MaHangMuc", "hm.MaHangMuc" },
            { "TenHangMuc", "hm.TenHangMuc" },
            { "IsDeleted", "hm.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, mapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, mapping, "hm.Id DESC");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                hm.Id, 
                hm.MaHangMuc, 
                hm.TenHangMuc, 
                hm.MoTa, 
                hm.ThoiGianUocTinhPhut, 
                hm.ChiPhiUocTinh, 
                hm.ChecklistTieuChuan
            FROM HangMucBaoTri hm
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<HangMucBaoTriReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => {
            List<string> checklist = new();
            if (!string.IsNullOrWhiteSpace(r.ChecklistTieuChuan))
            {
                try
                {
                    checklist = System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.ChecklistTieuChuan) ?? new();
                }
                catch
                {
                    // Ignore parse errors
                }
            }
            return new HangMucBaoTriResponse
            {
                Id = r.Id,
                MaHangMuc = r.MaHangMuc,
                TenHangMuc = r.TenHangMuc,
                MoTa = r.MoTa,
                ThoiGianUocTinhPhut = r.ThoiGianUocTinhPhut,
                ChiPhiUocTinh = r.ChiPhiUocTinh,
                ChecklistTieuChuan = checklist
            };
        }).ToList();

        return new PagedResult<HangMucBaoTriResponse>
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
