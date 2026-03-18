using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.PhuongTien.DTOs;
using HeThongChungCu.Application.Features.PhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

internal sealed class PhuongTienDapperRepository : IPhuongTienDapperRepository
{
    private readonly AppDbContext _dbContext;

    public PhuongTienDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<PhuongTienResponse>> LayDSPhuongTienTrongChungCu(
        LayDSPhuongTienTrongChungCuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "p.Id" },
            { "CanHoId", "p.CanHoId" },
            { "MaCanHo", "c.MaCanHo" },
            { "MaTang", "t.MaTang" },
            { "MaToaNha", "tn.MaToaNha" },
            { "TenPhuongTien", "p.TenPhuongTien" },
            { "LoaiPhuongTienId", "p.LoaiPhuongTienId" },
            { "BienSo", "p.BienSo" },
            { "MauXe", "p.MauXe" },
            { "TrangThaiPhuongTienId", "p.TrangThaiPhuongTienId" },
            { "ToaNhaId", "tn.Id" },
            { "TangId", "t.Id" },
            { "IsDeleted", "p.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                p.Id,
                c.MaCanHo,
                t.MaTang,
                tn.MaToaNha,
                p.TenPhuongTien,
                p.LoaiPhuongTienId,
                p.BienSo,
                p.MauXe,
                p.TrangThaiPhuongTienId
            FROM PhuongTiens p
            LEFT JOIN CanHos c ON c.Id = p.CanHoId AND c.IsDeleted = 0
            LEFT JOIN Tangs t ON t.Id = c.TangId AND t.IsDeleted = 0
            LEFT JOIN ToaNhas tn ON tn.Id = t.ToaNhaId AND tn.IsDeleted = 0
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<GetListPhuongTienReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new PhuongTienResponse
        {
            Id = r.Id,
            MaToaNha = r.MaToaNha,
            MaTang = r.MaTang,
            MaCanHo = r.MaCanHo,
            TenPhuongTien = r.TenPhuongTien,
            LoaiPhuongTienId = r.LoaiPhuongTienId,
            TenLoaiPhuongTien = LoaiPhuongTien.FromValue(r.LoaiPhuongTienId)?.Name ?? string.Empty,
            BienSo = r.BienSo,
            MauXe = r.MauXe,
            TrangThaiPhuongTienId = r.TrangThaiPhuongTienId,
            TenTrangThaiPhuongTien = TrangThaiPhuongTien.FromValue(r.TrangThaiPhuongTienId)?.Name ?? string.Empty
        }).ToList();

        return new PagedResult<PhuongTienResponse>
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
