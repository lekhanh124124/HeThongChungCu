using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class CanHoDapperRepository : ICanHoDapperRepository
{
    private readonly AppDbContext _dbContext;
    public CanHoDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CanHoDetailResponse>> GetAllAsync(
        GetListCanHoSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(CanHo.Id), "c.Id" },
            { nameof(CanHo.MaCanHo), "c.MaCanHo" },
            { nameof(CanHo.DienTich), "c.DienTich" },
            { nameof(CanHo.SoPhongNgu), "c.SoPhongNgu" },
            { nameof(CanHo.SoPhongTam), "c.SoPhongTam" },
            { nameof(CanHo.TinhTrangCanHoId), "c.TinhTrangCanHoId" },
            { nameof(CanHo.TangId), "c.TangId" },
            { nameof(Tang.TenTang), "t.TenTang" },
            { nameof(CanHo.TenCanHo), "c.TenCanHo" },
            { nameof(CanHo.LoaiCanHoId), "c.LoaiCanHoId" },
            { nameof(CanHo.IsDeleted), "c.IsDeleted" },

        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, nameof(CanHo.Id));
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                c.Id,
                c.MaCanHo,
                c.TenCanHo,
                c.TangId,
                t.TenTang,
                c.DienTich,
                c.SoPhongNgu,
                c.SoPhongTam,
                c.LoaiCanHoId,
                c.TinhTrangCanHoId
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<GetListCanHoReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiMap = LoaiCanHo.ToDictionary();
        var tinhTrangMap = TinhTrangCanHo.ToDictionary();

        var items = rows.Select(r => new CanHoDetailResponse
        {
            Id = r.Id,
            MaCanHo = r.MaCanHo,
            TenCanHo = r.TenCanHo,
            TangId = r.TangId,
            TenTang = r.TenTang,
            DienTich = r.DienTich,
            SoPhongNgu = r.SoPhongNgu,
            SoPhongTam = r.SoPhongTam,
            LoaiCanHoId = r.LoaiCanHoId,
            TinhTrangCanHoId = r.TinhTrangCanHoId,
            TenLoaiCanHo = loaiMap.GetValueOrDefault(r.LoaiCanHoId, string.Empty),
            TenTinhTrangCanHo = tinhTrangMap.GetValueOrDefault(r.TinhTrangCanHoId, string.Empty)
        }).ToList();

        return new PagedResult<CanHoDetailResponse>
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

    public async Task<CanHoResponse?> GetByIdAsync(GetCanHoByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(CanHo.Id), "c.Id" },
            { "CanHoIsDeleted", "c.IsDeleted" },
            { "TangIsDeleted", "t.IsDeleted" },
            { "QuanHeCuTruIsDeleted", "q.IsDeleted"  },
            { "UserIsDeleted", "u.IsDeleted" },
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT 
                c.Id, 
                c.TangId, 
                t.TenTang, 
                c.MaCanHo, 
                c.DienTich, 
                c.SoPhongNgu, 
                c.SoPhongTam, 
                c.LoaiCanHoId, 
                c.TinhTrangCanHoId,
                q.Id AS QuanHeCuTruId, 
                q.CanHoId, 
                q.UserId, 
                u.LastName + ' ' + u.FirstName AS FullName,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau, 
                q.NgayKetThuc, 
                q.IsKetThuc
            FROM CanHos c
            INNER JOIN Tangs t ON t.Id = c.TangId
            LEFT JOIN QuanHeCuTrus q ON q.CanHoId = c.Id
            LEFT JOIN Users u ON u.Id = q.UserId
            {sqlWhere};
            """;

        var rows = (await connection.QueryAsync<GetCanHoByIdReadModel>(sql, parameters)).ToList();

        if (!rows.Any())
            return null;

        var firstRow = rows.First();
        var canHo = new CanHoResponse
        {
            Id = firstRow.Id,
            TangId = firstRow.TangId,
            TenTang = firstRow.TenTang,
            MaCanHo = firstRow.MaCanHo,
            DienTich = firstRow.DienTich,
            SoPhongNgu = firstRow.SoPhongNgu,
            SoPhongTam = firstRow.SoPhongTam,
            LoaiCanHoId = firstRow.LoaiCanHoId,
            TinhTrangCanHoId = firstRow.TinhTrangCanHoId,
            TenLoaiCanHo = LoaiCanHo.ToDictionary().GetValueOrDefault(firstRow.LoaiCanHoId, string.Empty),
            TenTinhTrangCanHo = TinhTrangCanHo.ToDictionary().GetValueOrDefault(firstRow.TinhTrangCanHoId, string.Empty)
        };

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var quanHeCuTrus = rows
            .Where(r => r.QuanHeCuTruId.HasValue)
            .Select(r => new QuanHeCuTruDetailResponse
            {
                Id = r.QuanHeCuTruId!.Value,
                CanHoId = r.Id,
                UserId = r.UserId!.Value,
                FullName = r.FullName ?? string.Empty,
                LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId!.Value,
                TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId!.Value, string.Empty),
                NgayBatDau = r.NgayBatDau!.Value,
                NgayKetThuc = r.NgayKetThuc,
                IsKetThuc = r.IsKetThuc!.Value
            })
            .ToList();

        canHo.QuanHeCuTrus = quanHeCuTrus;

        return canHo;
    }
}
