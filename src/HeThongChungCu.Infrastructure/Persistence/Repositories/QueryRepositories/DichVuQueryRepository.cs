using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class DichVuQueryRepository : IDichVuQueryRepository
{
    private readonly AppDbContext _dbContext;

    public DichVuQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DichVuResponse>> GetListAsync(
        GetListDichVuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "IsDeleted", "dv.IsDeleted" },
            { "LoaiDichVuId", "dv.LoaiDichVuId" },
            { "TenDichVu", "dv.TenDichVu" },
            { "MaDichVu", "dv.MaDichVu" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var joins = new List<JoinDefinition>
        {
            new("TepTaiLieu", "tp", "tp.Id = dv.IconId", JoinType.Left, false)
        };
        var sqlJoin = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   dv.Id, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.MoTa, dv.IsBatBuoc, dv.TrangThaiId,
                   tp.FileUrl AS IconUrl
            FROM DichVu dv
            {sqlJoin}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DichVuResponse
        {
            Id = (int)r.Id,
            MaDichVu = (string)r.MaDichVu,
            TenDichVu = (string)r.TenDichVu,
            LoaiDichVuId = (int)r.LoaiDichVuId,
            LoaiDichVuTen = LoaiDichVu.FromValue((int)r.LoaiDichVuId)!.Name,
            DonViTinh = (string)r.DonViTinh,
            MoTa = (string?)r.MoTa,
            IsBatBuoc = (bool)r.IsBatBuoc,
            TrangThaiDichVuId = (int)r.TrangThaiId,
            TrangThaiDichVuTen = TrangThaiDichVu.FromValue((int)r.TrangThaiId)!.Name,
            IconUrl = (string?)r.IconUrl
        }).ToList();

        return new PagedResult<DichVuResponse>
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

    public async Task<DichVuDetailResponse?> GetByIdAsync(
        GetDichVuByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dv.Id" },
            { "IsDeleted", "dv.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var joins = new List<JoinDefinition>
        {
            new("KhungGioDichVu", "kg", "kg.DichVuId = dv.Id AND kg.IsActive = 1", JoinType.Left, true),
            new("BangGia", "bg", "bg.DichVuId = dv.Id AND bg.IsActive = 1", JoinType.Left, true),
            new("TepTaiLieu", "tp", "tp.Id = dv.IconId", JoinType.Left, false),
            new("ChiTietGiaLuyTien", "lt", "lt.BangGiaId = bg.Id", JoinType.Left, false),
            new("ChiTietGiaKhungGio", "ckg", "ckg.BangGiaId = bg.Id", JoinType.Left, false),
            new("KhungGioDichVu", "kg_detail", "ckg.KhungGioId = kg_detail.Id", JoinType.Left, true),
            new("ChiTietGiaLoaiCanHo", "clch", "clch.BangGiaId = bg.Id", JoinType.Left, false)
        };
        var sqlJoin = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT 
                dv.Id, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.MoTa, dv.IsBatBuoc, dv.SoLuongToiDa, dv.TrangThaiId,
                tp.FileUrl AS IconUrl,
                
                kg.Id AS KhungGioId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan,
                
                bg.Id AS BangGiaId, bg.TenBangGia, bg.NgayApDung, bg.NgayKetThuc, bg.LoaiDinhGiaId, bg.IsActive, bg.DonGia,
                
                lt.Id AS GiaLuyTienId, lt.TuMuc, lt.DenMuc, lt.DonGia AS DonGiaLuyTien,
                
                ckg.Id AS GiaKhungGioId, ckg.KhungGioId AS KhungGioId_Detail, ckg.DonGia AS DonGiaKhungGio, kg_detail.TenKhungGio AS TenKhungGio_Detail,
                
                clch.Id AS GiaLoaiCanHoId, clch.LoaiCanHoId, clch.DonGia AS DonGiaLoaiCanHo
            FROM DichVu dv
            {sqlJoin}
            {sqlWhere};
            """;

        var rows = (await connection.QueryAsync<DichVuDetailReadModel>(sql, parameters)).ToList();
        if (!rows.Any()) return null;

        var first = rows.First();
        var result = new DichVuDetailResponse
        {
            Id = first.Id,
            MaDichVu = first.MaDichVu,
            TenDichVu = first.TenDichVu,
            LoaiDichVuId = first.LoaiDichVuId,
            LoaiDichVuTen = LoaiDichVu.FromValue(first.LoaiDichVuId)!.Name,
            DonViTinh = first.DonViTinh,
            MoTa = first.MoTa,
            IsBatBuoc = first.IsBatBuoc,
            SoLuongToiDa = first.SoLuongToiDa,
            TrangThaiDichVuId = first.TrangThaiId,
            TrangThaiDichVuTen = TrangThaiDichVu.FromValue(first.TrangThaiId)!.Name,
            IconUrl = first.IconUrl,
            KhungGioDichVu = rows
                .Where(r => r.KhungGioId.HasValue)
                .GroupBy(r => r.KhungGioId!.Value)
                .Select(g =>
                {
                    var r = g.First();
                    return new KhungGioDichVuResponse
                    {
                        Id = r.KhungGioId!.Value,
                        DichVuId = first.Id,
                        GioBatDau = r.GioBatDau!.Value,
                        GioKetThuc = r.GioKetThuc!.Value,
                        TenKhungGio = r.TenKhungGio!,
                        NgayTrongTuan = r.NgayTrongTuan
                    };
                })
                .ToList(),
            BangGia = rows
                .Where(r => r.BangGiaId.HasValue)
                .GroupBy(r => r.BangGiaId!.Value)
                .Select(g =>
                {
                    var r = g.First();
                    var bgResponse = new BangGiaResponse
                    {
                        Id = g.Key,
                        DichVuId = first.Id,
                        TenBangGia = r.TenBangGia!,
                        NgayApDung = r.NgayApDung!.Value.DateTime,
                        NgayKetThuc = r.NgayKetThuc?.DateTime,
                        LoaiDinhGiaId = r.LoaiDinhGiaId!.Value,
                        LoaiDinhGiaTen = LoaiDinhGia.FromValue(r.LoaiDinhGiaId!.Value)?.Name ?? string.Empty,
                        DonGia = r.DonGia,
                        IsActive = r.IsActive ?? false
                    };

                    // Populate detail collections using unique Ids to avoid duplicates from joins
                    bgResponse.GiaLuyTiens.AddRange(g
                        .Where(x => x.GiaLuyTienId.HasValue)
                        .GroupBy(x => x.GiaLuyTienId)
                        .Select(ltGroup =>
                        {
                            var lt = ltGroup.First();
                            return new ChiTietGiaLuyTienResponse
                            {
                                Id = lt.GiaLuyTienId!.Value,
                                BangGiaId = g.Key,
                                TuMuc = lt.TuMuc!.Value,
                                DenMuc = lt.DenMuc,
                                DonGia = lt.DonGiaLuyTien!.Value
                            };
                        }));

                    bgResponse.GiaKhungGios.AddRange(g
                        .Where(x => x.GiaKhungGioId.HasValue)
                        .GroupBy(x => x.GiaKhungGioId)
                        .Select(ckgGroup =>
                        {
                            var ckg = ckgGroup.First();
                            return new ChiTietGiaKhungGioResponse
                            {
                                Id = ckg.GiaKhungGioId!.Value,
                                BangGiaId = g.Key,
                                KhungGioId = ckg.KhungGioId_Detail!.Value,
                                TenKhungGio = ckg.TenKhungGio_Detail ?? string.Empty,
                                DonGia = ckg.DonGiaKhungGio!.Value
                            };
                        }));

                    bgResponse.GiaLoaiCanHos.AddRange(g
                        .Where(x => x.GiaLoaiCanHoId.HasValue)
                        .GroupBy(x => x.GiaLoaiCanHoId)
                        .Select(clchGroup =>
                        {
                            var clch = clchGroup.First();
                            return new ChiTietGiaLoaiCanHoResponse
                            {
                                Id = clch.GiaLoaiCanHoId!.Value,
                                BangGiaId = g.Key,
                                LoaiCanHoId = clch.LoaiCanHoId,
                                LoaiCanHoTen = clch.LoaiCanHoId.HasValue ? LoaiCanHo.FromValue(clch.LoaiCanHoId.Value)?.Name : null,
                                DonGia = clch.DonGiaLoaiCanHo!.Value
                            };
                        }));

                    return bgResponse;
                })
                .FirstOrDefault()!
        };

        return result;
    }

    public async Task<PagedResult<KhungGioDichVuResponse>> GetListKhungGioAsync(
        HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu.GetListKhungGioDichVuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "kg.Id" },
            { "DichVuId", "kg.DichVuId" },
            { "TenKhungGio", "kg.TenKhungGio" },
            { "IsDeleted", "kg.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   kg.Id, kg.DichVuId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan
            FROM KhungGioDichVu kg
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new KhungGioDichVuResponse
        {
            Id = (int)r.Id,
            DichVuId = (int)r.DichVuId,
            GioBatDau = (TimeSpan)r.GioBatDau,
            GioKetThuc = (TimeSpan)r.GioKetThuc,
            TenKhungGio = (string)r.TenKhungGio,
            NgayTrongTuan = (int?)r.NgayTrongTuan
        }).ToList();

        return new PagedResult<KhungGioDichVuResponse>
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

    public async Task<KhungGioDichVuResponse?> GetKhungGioByIdAsync(
        HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById.GetKhungGioDichVuByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "kg.Id" },
            { "IsDeleted", "kg.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT kg.Id, kg.DichVuId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan
            FROM KhungGioDichVu kg
            {sqlWhere};
            """;

        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters);
        if (row == null) return null;

        return new KhungGioDichVuResponse
        {
            Id = (int)row.Id,
            DichVuId = (int)row.DichVuId,
            GioBatDau = (TimeSpan)row.GioBatDau,
            GioKetThuc = (TimeSpan)row.GioKetThuc,
            TenKhungGio = (string)row.TenKhungGio,
            NgayTrongTuan = (int?)row.NgayTrongTuan
        };
    }

    public async Task<PagedResult<BangGiaResponse>> GetListBangGiaAsync(
        HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia.GetListBangGiaSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "bg.Id" },
            { "TenBangGia", "bg.TenBangGia" },
            { "NgayApDung", "bg.NgayApDung" },
            { "NgayKetThuc", "bg.NgayKetThuc" },
            { "LoaiDinhGiaId", "bg.LoaiDinhGiaId" },
            { "IsActive", "bg.IsActive" },
            { "DichVuId", "bg.DichVuId" },
            { "IsDeleted", "bg.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   bg.Id, bg.TenBangGia, bg.NgayApDung, bg.NgayKetThuc, bg.LoaiDinhGiaId, bg.IsActive, bg.DonGia, bg.DichVuId
            FROM BangGia bg
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new BangGiaResponse
        {
            Id = (int)r.Id,
            DichVuId = (int)r.DichVuId,
            TenBangGia = (string)r.TenBangGia,
            NgayApDung = r.NgayApDung is DateTimeOffset dto1 ? dto1.UtcDateTime : (DateTime)r.NgayApDung,
            NgayKetThuc = r.NgayKetThuc is DateTimeOffset dto2 ? dto2.UtcDateTime : (DateTime?)r.NgayKetThuc,
            LoaiDinhGiaId = (int)r.LoaiDinhGiaId,
            LoaiDinhGiaTen = LoaiDinhGia.FromValue((int)r.LoaiDinhGiaId)?.Name ?? string.Empty,
            DonGia = (decimal?)r.DonGia,
            IsActive = (bool)r.IsActive
        }).ToList();

        return new PagedResult<BangGiaResponse>
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

    public async Task<BangGiaResponse?> GetBangGiaByIdAsync(
        HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById.GetBangGiaByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "bg.Id" },
            { "IsDeleted", "bg.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec as IQuerySpecification, columnMapping);

        // Fetch BangGia
        var sqlBangGia = $"""
            SELECT bg.Id, bg.TenBangGia, bg.NgayApDung, bg.NgayKetThuc, bg.LoaiDinhGiaId, bg.IsActive, bg.DonGia, bg.DichVuId
            FROM BangGia bg
            {sqlWhere};
            """;

        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sqlBangGia, parameters);
        if (row == null) return null;

        var bangGia = new BangGiaResponse
        {
            Id = (int)row.Id,
            DichVuId = (int)row.DichVuId,
            TenBangGia = (string)row.TenBangGia,
            NgayApDung = row.NgayApDung is DateTimeOffset dto1 ? dto1.UtcDateTime : (DateTime)row.NgayApDung,
            NgayKetThuc = row.NgayKetThuc is DateTimeOffset dto2 ? dto2.UtcDateTime : (DateTime?)row.NgayKetThuc,
            LoaiDinhGiaId = (int)row.LoaiDinhGiaId,
            LoaiDinhGiaTen = LoaiDinhGia.FromValue((int)row.LoaiDinhGiaId)?.Name ?? string.Empty,
            DonGia = (decimal?)row.DonGia,
            IsActive = (bool)row.IsActive
        };

        // Fetch Details based on type
        if (bangGia.LoaiDinhGiaId == LoaiDinhGia.LuyTien.Value)
        {
            var sqlLuyTien = $"""
                SELECT Id, TuMuc, DenMuc, DonGia
                FROM ChiTietGiaLuyTien
                WHERE BangGiaId = @BangGiaId
                ORDER BY TuMuc;
                """;
            var details = await connection.QueryAsync<ChiTietGiaLuyTienResponse>(sqlLuyTien, new { BangGiaId = bangGia.Id });
            bangGia = bangGia with { GiaLuyTiens = details.ToList() };
        }
        else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio.Value)
        {
            var sqlKhungGio = $"""
                SELECT ct.Id, ct.KhungGioId, ct.DonGia, kg.TenKhungGio
                FROM ChiTietGiaKhungGio ct
                JOIN KhungGioDichVu kg ON ct.KhungGioId = kg.Id
                WHERE ct.BangGiaId = @BangGiaId;
                """;
            var details = await connection.QueryAsync<ChiTietGiaKhungGioResponse>(sqlKhungGio, new { BangGiaId = bangGia.Id });
            bangGia = bangGia with { GiaKhungGios = details.ToList() };
        }
        else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoDienTich.Value)
        {
            var sqlLoaiCanHo = $"""
                SELECT Id, LoaiCanHoId, DonGia
                FROM ChiTietGiaLoaiCanHo
                WHERE BangGiaId = @BangGiaId;
                """;
            var details = (await connection.QueryAsync<dynamic>(sqlLoaiCanHo, new { BangGiaId = bangGia.Id })).Select(r => new ChiTietGiaLoaiCanHoResponse
            {
                Id = (int)r.Id,
                LoaiCanHoId = (int?)r.LoaiCanHoId,
                LoaiCanHoTen = r.LoaiCanHoId != null ? LoaiCanHo.FromValue((int)r.LoaiCanHoId)?.Name : null,
                DonGia = (decimal)r.DonGia
            }).ToList();
            bangGia = bangGia with { GiaLoaiCanHos = details };
        }

        return bangGia;
    }
}
