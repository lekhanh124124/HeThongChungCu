using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienList;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class NhanVienQueryRepository : INhanVienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public NhanVienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NhanVienResponse?> GetByIdAsync(GetNhanVienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "IsDeleted", "nv.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var joins = new[]
        {
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Type: JoinType.Inner)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT 
                nv.Id,
                nv.NguoiDungId,
                u.Ho + ' ' + u.Ten AS HoTen,
                u.SoDienThoai,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu
            FROM NhanVien nv
            {sqlJoins}
            {sqlWhere}
            """;

        var result = await connection.QueryFirstOrDefaultAsync<NhanVienReadModel>(sql, parameters);

        if (result == null) return null;

        return MapToResponse(result);
    }

    public async Task<PagedResult<NhanVienResponse>> GetListAsync(GetNhanVienListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "MaNhanVien", "nv.MaNhanVien" },
            { "LoaiNhanVienId", "nv.LoaiNhanVienId" },
            { "TrangThaiNhanVienId", "nv.TrangThaiNhanVienId" },
            { "IsDeleted", "nv.IsDeleted" },
            { "Keyword", "nv.MaNhanVien + ' ' + u.Ho + ' ' + u.Ten + ' ' + u.SoDienThoai" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var joins = new[]
        {
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Type: JoinType.Inner)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                nv.Id,
                nv.NguoiDungId,
                u.Ho + ' ' + u.Ten AS HoTen,
                u.SoDienThoai,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu
            FROM NhanVien nv
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<NhanVienReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(MapToResponse).ToList();

        return new PagedResult<NhanVienResponse>
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

    private static NhanVienResponse MapToResponse(NhanVienReadModel model)
    {
        return new NhanVienResponse
        {
            Id = model.Id,
            NguoiDungId = model.NguoiDungId,
            HoTen = model.HoTen,
            SoDienThoai = model.SoDienThoai,
            LoaiNhanVienId = model.LoaiNhanVienId,
            TenLoaiNhanVien = LoaiNhanVien.FromValue(model.LoaiNhanVienId)?.Name ?? string.Empty,
            TrangThaiNhanVienId = model.TrangThaiNhanVienId,
            TenTrangThaiNhanVien = TrangThaiNhanVien.FromValue(model.TrangThaiNhanVienId)?.Name ?? string.Empty,
            MaNhanVien = model.MaNhanVien,
            NgayVaoLam = model.NgayVaoLam,
            NgayNghiLam = model.NgayNghiLam,
            GhiChu = model.GhiChu
        };
    }
}
