using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;
using HeThongChungCu.Application.Features.Tang.Queries.GetTangById;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class TangDapperRepository : ITangDapperRepository
{
    private readonly AppDbContext _dbContext;

    public TangDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TangDetailResponse>> GetAllAsync(
        GetListTangSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "MaTang", "t.MaTang" },
            { "TenTang", "t.TenTang" },
            { "ToaNhaId", "t.ToaNhaId" },
            { "LoaiTangId", "t.LoaiTangId" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, nameof(Tang.Id));
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                t.Id,
                t.MaTang,
                t.TenTang,
                t.LoaiTangId,
                t.ToaNhaId,
                tn.TenToaNha
            FROM Tangs t
            INNER JOIN ToaNhas tn ON tn.Id = t.ToaNhaId
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;


        var rows = (await connection.QueryAsync<GetListTangReadModel>(sql, parameters)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiTangMap = LoaiTang.ToDictionary();

        var items = rows.Select(r => new TangDetailResponse
        {
            Id = r.Id,
            MaTang = r.MaTang,
            TenTang = r.TenTang,
            LoaiTangId = r.LoaiTangId,
            ToaNhaId = r.ToaNhaId,
            TenToaNha = r.TenToaNha,
            TenLoaiTang = loaiTangMap.GetValueOrDefault(r.LoaiTangId, string.Empty)
        }).ToList();

        return new PagedResult<TangDetailResponse>
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

    public async Task<TangResponse?> GetByIdAsync(
        GetTangByIdSpecification spec, 
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "TangIsDeleted", "t.IsDeleted" },
            { "ToaNhaIsDeleted", "tn.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT t.Id, t.MaTang, t.TenTang, t.LoaiTangId, t.ToaNhaId, tn.TenToaNha,
                   c.Id AS CanHoId, t.TenTang AS TenTangColumn, c.MaCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM Tangs t
            INNER JOIN ToaNhas tn ON tn.Id = t.ToaNhaId
            LEFT JOIN CanHos c ON c.TangId = t.Id AND c.IsDeleted = 0
            {sqlWhere};
            """;

        var rows = (await connection.QueryAsync<GetTangByIdReadModel>(sql, parameters)).ToList();

        if (!rows.Any())
            return null;

        var firstRow = rows.First();
        var tang = new TangResponse
        {
            Id = firstRow.Id,
            MaTang = firstRow.MaTang,
            TenTang = firstRow.TenTang,
            LoaiTangId = firstRow.LoaiTangId,
            ToaNhaId = firstRow.ToaNhaId,
            TenToaNha = firstRow.TenToaNha,
            TenLoaiTang = LoaiTang.ToDictionary().GetValueOrDefault(firstRow.LoaiTangId, string.Empty)
        };

        var loaiCanHoDict = LoaiCanHo.ToDictionary();
        var tinhTrangCanHoDict = TrangThaiCanHo.ToDictionary();

        var canHos = rows
            .Where(r => r.CanHoId.HasValue)
            .Select(r => new CanHoDetailResponse
            {
                Id = r.CanHoId!.Value,
                TangId = firstRow.Id,
                TenTang = r.TenTangColumn ?? firstRow.TenTang,
                MaCanHo = r.MaCanHo ?? string.Empty,
                TenCanHo = r.MaCanHo ?? string.Empty,
                DienTich = r.DienTich ?? 0,
                SoPhongNgu = r.SoPhongNgu ?? 0,
                SoPhongTam = r.SoPhongTam ?? 0,
                LoaiCanHoId = r.LoaiCanHoId ?? 0,
                TinhTrangCanHoId = r.TinhTrangCanHoId ?? 0,
                TenLoaiCanHo = loaiCanHoDict.GetValueOrDefault(r.LoaiCanHoId ?? 0, string.Empty),
                TenTinhTrangCanHo = tinhTrangCanHoDict.GetValueOrDefault(r.TinhTrangCanHoId ?? 0, string.Empty)
            })
            .ToList();

        tang.CanHos = canHos;

        return tang;
    }
}
