using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

internal sealed class PhuongTienQueryRepository : IPhuongTienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public PhuongTienQueryRepository(AppDbContext dbContext)
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
        var joins = new[]
        {
            new JoinDefinition("CanHo", "c", "c.Id = p.CanHoId"),
            new JoinDefinition("Tang", "t", "t.Id = c.TangId"),
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId")
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                p.Id,
                c.Id AS CanHoId,
                c.MaCanHo,
                t.MaTang,
                tn.MaToaNha,
                p.TenPhuongTien,
                p.LoaiPhuongTienId,
                p.BienSo,
                p.MauXe,
                p.TrangThaiPhuongTienId
            FROM PhuongTien p
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<GetListPhuongTienReadModel>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiPhuongTienMap = LoaiPhuongTien.ToDictionary();
        var trangThaiPhuongTienMap = TrangThaiPhuongTien.ToDictionary();

        var items = rows.Select(r => new PhuongTienResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            MaToaNha = r.MaToaNha,
            MaTang = r.MaTang,
            MaCanHo = r.MaCanHo,
            TenPhuongTien = r.TenPhuongTien,
            LoaiPhuongTienId = r.LoaiPhuongTienId,
            TenLoaiPhuongTien = loaiPhuongTienMap.GetValueOrDefault(r.LoaiPhuongTienId, string.Empty),
            BienSo = r.BienSo,
            MauXe = r.MauXe,
            TrangThaiPhuongTienId = r.TrangThaiPhuongTienId,
            TenTrangThaiPhuongTien = trangThaiPhuongTienMap.GetValueOrDefault(r.TrangThaiPhuongTienId, string.Empty)
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

    public async Task<PhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var joins = new[]
        {
            new JoinDefinition("CanHo", "c", "c.Id = p.CanHoId"),
            new JoinDefinition("Tang", "t", "t.Id = c.TangId"),
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId")
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = """
            SELECT
                p.Id,
                c.Id AS CanHoId,
                c.MaCanHo,
                t.MaTang,
                tn.MaToaNha,
                p.TenPhuongTien,
                p.LoaiPhuongTienId,
                p.BienSo,
                p.MauXe,
                p.TrangThaiPhuongTienId
            FROM PhuongTien p
            {sqlJoins}
            WHERE p.Id = @Id AND p.IsDeleted = 0;

            SELECT
                Id,
                PhuongTienId,
                MaThe,
                NgayBatDau,
                NgayKetThuc,
                TrangThaiId AS TrangThaiThePhuongTienId
            FROM ThePhuongTien
            WHERE PhuongTienId = @Id AND IsDeleted = 0;

            SELECT
                t.Id AS FileId,
                t.FileName,
                t.FileUrl,
                t.ContentType
            FROM TepTaiLieu t
            WHERE t.PhuongTienId = @Id AND t.LoaiTepTaiLieu = 'TepPhuongTien' AND t.IsDeleted = 0;
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
        var phuongTien = await multi.ReadFirstOrDefaultAsync<GetListPhuongTienReadModel>();
        
        if (phuongTien == null)
            return null;

        var cards = (await multi.ReadAsync<ThePhuongTienResponse>()).ToList();
        foreach (var card in cards)
        {
            card.TenTrangThaiThePhuongTien = TrangThaiThePhuongTien.FromValue(card.TrangThaiThePhuongTienId)?.Name ?? string.Empty;
        }

        return new PhuongTienResponse
        {
            Id = phuongTien.Id,
            CanHoId = phuongTien.CanHoId,
            MaToaNha = phuongTien.MaToaNha,
            MaTang = phuongTien.MaTang,
            MaCanHo = phuongTien.MaCanHo,
            TenPhuongTien = phuongTien.TenPhuongTien,
            LoaiPhuongTienId = phuongTien.LoaiPhuongTienId,
            TenLoaiPhuongTien = LoaiPhuongTien.FromValue(phuongTien.LoaiPhuongTienId)?.Name ?? string.Empty,
            BienSo = phuongTien.BienSo,
            MauXe = phuongTien.MauXe,
            TrangThaiPhuongTienId = phuongTien.TrangThaiPhuongTienId,
            TenTrangThaiPhuongTien = TrangThaiPhuongTien.FromValue(phuongTien.TrangThaiPhuongTienId)?.Name ?? string.Empty,
            ThePhuongTiens = cards,
            HinhAnhPhuongTiens = (await multi.ReadAsync<UploadFileResponse>()).ToList()
        };
    }
}
