using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDangKyDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;

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
            { "LoaiDichVuId", "dv.LoaiDichVuId" },
            { "TenDichVu", "dv.TenDichVu" },
            { "MaDichVu", "dv.MaDichVu" },
            { "IsBatBuoc", "dv.IsBatBuoc" },
            { "TrangThaiDichVuId", "dv.TrangThaiId" },
            { "IsDeleted", "dv.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var tepDuLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "LoaiTepTaiLieu", "tp.LoaiTepId" },
            { "TepDuLieuIsDeleted", "tp.IsDeleted" }
        };

        var hopDongMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "HopDongDoiTacId", "hd.Id" },
            { "TrangThaiHopDongId", "hd.TrangThaiHopDongId" }
        };

        var doiTacMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DoiTacId", "dt.Id" }
        };

        var sqlJoin = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "tp", "tp.Id = dv.IconId", Mapping: tepDuLieuMapping),
            new JoinDefinition("HopDongDoiTac", "hd", "hd.DichVuId = dv.Id", Mapping: hopDongMapping),
            new JoinDefinition("DoiTac", "dt", "dt.Id = hd.DoiTacId", Mapping: doiTacMapping)
        ], parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   dv.Id, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.MoTa, dv.IsBatBuoc, dv.SoLuongToiDa, dv.TrangThaiId,
                   tp.FileUrl AS IconUrl,
                   hd.Id AS HopDongDoiTacId, hd.SoHopDong, dt.TenDoiTac, hd.TrangThaiHopDongId
            FROM DichVu dv
            {sqlJoin}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<DichVuReadModel>(sql, parameters, transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DichVuResponse
        {
            Id = r.Id,
            MaDichVu = r.MaDichVu,
            TenDichVu = r.TenDichVu,
            LoaiDichVuId = r.LoaiDichVuId,
            LoaiDichVuTen = LoaiDichVu.FromValue(r.LoaiDichVuId)?.Name ?? string.Empty,
            DonViTinh = r.DonViTinh,
            MoTa = r.MoTa,
            IsBatBuoc = r.IsBatBuoc,
            TrangThaiDichVuId = r.TrangThaiId,
            TrangThaiDichVuTen = TrangThaiDichVu.FromValue(r.TrangThaiId)?.Name ?? string.Empty,
            IconUrl = r.IconUrl,
            HopDongDoiTacId = r.HopDongDoiTacId,
            SoHopDong = r.SoHopDong,
            TenDoiTac = r.TenDoiTac,
            TrangThaiHopDongId = r.TrangThaiHopDongId,
            TrangThaiHopDongTen = r.TrangThaiHopDongId.HasValue ? TrangThaiHopDong.FromValue(r.TrangThaiHopDongId.Value)?.Name : null
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

        // Mapping cơ bản cho root
        var dichVuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dv.Id" },
            { "IsDeleted", "dv.IsDeleted" }
        };

        var khungGioMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "KhungGioIsActive", "kg.IsActive" },
            { "KhungGioIsDeleted", "kg.IsDeleted" }
        };

        var bangGiaMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "BangGiaIsActive", "bg.IsActive" },
            { "BangGiaIsDeleted", "bg.IsDeleted" }
        };

        var tepDuLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "LoaiTepTaiLieu", "tp.LoaiTepId" },
            { "TepDuLieuIsDeleted", "tp.IsDeleted" }
        };

        var hopDongMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "HopDongDoiTacId", "hd.Id" },
            { "TrangThaiHopDongId", "hd.TrangThaiHopDongId" }
        };

        var doiTacMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DoiTacId", "dt.Id" }
        };

        var parameters = new DynamicParameters();

        // Build SQL Where cho Dịch vụ (Root)
        var sqlWhereDv = DapperQueryBuilder.BuildWhere(spec, dichVuMapping, parameters);

        // --- Demo JoinExplicitWithSpec: Tầng Application quyết định OnCondition ---

        var sqlJoinsDv = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "tp", "tp.Id = dv.IconId", Mapping: tepDuLieuMapping),
            new JoinDefinition("HopDongDoiTac", "hd", "hd.DichVuId = dv.Id", Mapping: hopDongMapping),
            new JoinDefinition("DoiTac", "dt", "dt.Id = hd.DoiTacId", Mapping: doiTacMapping)
        ], parameters);

        var sqlJoinsKg = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("KhungGioDichVu", "kg", "kg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: khungGioMapping)
        ], parameters);

        var sqlJoinsBg = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("BangGia", "bg", "bg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: bangGiaMapping)
        ], parameters);

        var sqlJoinsCtLuyTien = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("BangGia", "bg", "bg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: bangGiaMapping),
            new JoinDefinition("ChiTietGiaLuyTien", "ct", "ct.BangGiaId = bg.Id")
        ], parameters);

        var sqlJoinsCtKhungGio = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("BangGia", "bg", "bg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: bangGiaMapping),
            new JoinDefinition("KhungGioDichVu", "kg", "kg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: khungGioMapping),
            new JoinDefinition("ChiTietGiaKhungGio", "ct", "ct.BangGiaId = bg.Id AND ct.KhungGioId = kg.Id")
        ], parameters);

        var sqlJoinsCtLoaiCanHo = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("BangGia", "bg", "bg.DichVuId = dv.Id", Type: JoinType.Inner, Mapping: bangGiaMapping),
            new JoinDefinition("ChiTietGiaLoaiCanHo", "ct", "ct.BangGiaId = bg.Id")
        ], parameters);

        var sql = $"""
            -- Query 1: DichVu + Icon (N-1) + HopDong/DoiTac
            SELECT dv.Id, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.MoTa, dv.IsBatBuoc, dv.SoLuongToiDa, dv.TrangThaiId,
                   tp.FileUrl AS IconUrl,
                   hd.Id AS HopDongDoiTacId, hd.SoHopDong, dt.TenDoiTac, hd.TrangThaiHopDongId
            FROM DichVu dv
            {sqlJoinsDv}
            {sqlWhereDv};

            -- Query 2: KhungGioDichVu (1-N)
            SELECT kg.Id, kg.DichVuId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan, kg.IsActive
            FROM DichVu dv
            {sqlJoinsKg}
            {sqlWhereDv}; 

            -- Query 3: BangGia (1-N)
            SELECT bg.Id, bg.TenBangGia, bg.NgayApDung, bg.NgayKetThuc, bg.LoaiDinhGiaId, bg.IsActive, bg.DonGia, bg.DichVuId
            FROM DichVu dv
            {sqlJoinsBg}
            {sqlWhereDv};

            -- Query 4: ChiTietGiaLuyTien
            SELECT ct.Id, ct.TuMuc, ct.DenMuc, ct.DonGia, ct.BangGiaId
            FROM DichVu dv
            {sqlJoinsCtLuyTien}
            {sqlWhereDv}
            ORDER BY ct.TuMuc;

            -- Query 5: ChiTietGiaKhungGio
            SELECT ct.Id, ct.KhungGioId, ct.DonGia, kg.TenKhungGio, ct.BangGiaId
            FROM DichVu dv
            {sqlJoinsCtKhungGio}
            {sqlWhereDv};

            -- Query 6: ChiTietGiaLoaiCanHo
            SELECT ct.Id, ct.LoaiCanHoId, ct.DonGia, ct.BangGiaId
            FROM DichVu dv
            {sqlJoinsCtLoaiCanHo}
            {sqlWhereDv};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var first = await multi.ReadFirstOrDefaultAsync<DichVuDetailReadModel>();
        if (first == null) return null;

        var kgDetails = (await multi.ReadAsync<KhungGioDichVuReadModel>()).ToList();
        var bgRows = (await multi.ReadAsync<BangGiaReadModel>()).ToList();
        var luyTienDetails = (await multi.ReadAsync<ChiTietGiaLuyTienReadModel>()).ToList();
        var khungGioDetails = (await multi.ReadAsync<ChiTietGiaKhungGioReadModel>()).ToList();
        var loaiCanHoRows = (await multi.ReadAsync<ChiTietGiaLoaiCanHoReadModel>()).ToList();

        var result = new DichVuDetailResponse
        {
            Id = first.Id,
            MaDichVu = first.MaDichVu,
            TenDichVu = first.TenDichVu,
            LoaiDichVuId = first.LoaiDichVuId,
            LoaiDichVuTen = LoaiDichVu.FromValue(first.LoaiDichVuId)?.Name ?? string.Empty,
            DonViTinh = first.DonViTinh,
            MoTa = first.MoTa,
            IsBatBuoc = first.IsBatBuoc,
            SoLuongToiDa = first.SoLuongToiDa,
            TrangThaiDichVuId = first.TrangThaiId,
            TrangThaiDichVuTen = TrangThaiDichVu.FromValue(first.TrangThaiId)?.Name ?? string.Empty,
            IconUrl = first.IconUrl,
            HopDongDoiTacId = first.HopDongDoiTacId,
            SoHopDong = first.SoHopDong,
            TenDoiTac = first.TenDoiTac,
            TrangThaiHopDongId = first.TrangThaiHopDongId,
            TrangThaiHopDongTen = first.TrangThaiHopDongId.HasValue ? TrangThaiHopDong.FromValue(first.TrangThaiHopDongId.Value)?.Name : null,
            KhungGioDichVu = kgDetails
                .Select(kg => new KhungGioDichVuResponse
                {
                    Id = kg.Id,
                    DichVuId = kg.DichVuId,
                    GioBatDau = kg.GioBatDau,
                    GioKetThuc = kg.GioKetThuc,
                    TenKhungGio = kg.TenKhungGio,
                    NgayTrongTuan = kg.NgayTrongTuan,
                    IsActive = kg.IsActive
                }).ToList()
        };

        var bgRow = bgRows.FirstOrDefault();
        if (bgRow != null)
        {
            var bangGia = new BangGiaResponse
            {
                Id = bgRow.Id,
                DichVuId = bgRow.DichVuId,
                TenBangGia = bgRow.TenBangGia,
                NgayApDung = bgRow.NgayApDung.DateTime,
                NgayKetThuc = bgRow.NgayKetThuc?.DateTime,
                LoaiDinhGiaId = bgRow.LoaiDinhGiaId,
                LoaiDinhGiaTen = LoaiDinhGia.FromValue(bgRow.LoaiDinhGiaId)?.Name ?? string.Empty,
                LoaiDinhGiaCode = LoaiDinhGia.FromValue(bgRow.LoaiDinhGiaId)?.Code ?? string.Empty,
                DonGia = bgRow.DonGia,
                IsActive = bgRow.IsActive
            };

            if (bangGia.LoaiDinhGiaId == LoaiDinhGia.LuyTien.Value)
            {
                bangGia = bangGia with
                {
                    GiaLuyTiens = luyTienDetails
                        .Where(x => x.BangGiaId == bangGia.Id)
                        .Select(x => new ChiTietGiaLuyTienResponse
                        {
                            Id = x.Id,
                            TuMuc = x.TuMuc,
                            DenMuc = x.DenMuc,
                            DonGia = x.DonGia,
                            BangGiaId = x.BangGiaId
                        }).ToList()
                };
            }
            else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio.Value)
            {
                bangGia = bangGia with
                {
                    GiaKhungGios = khungGioDetails
                        .Where(x => x.BangGiaId == bangGia.Id)
                        .Select(x => new ChiTietGiaKhungGioResponse
                        {
                            Id = x.Id,
                            KhungGioId = x.KhungGioId,
                            DonGia = x.DonGia,
                            TenKhungGio = x.TenKhungGio,
                            BangGiaId = x.BangGiaId
                        }).ToList()
                };
            }
            else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoDienTich.Value)
            {
                var details = loaiCanHoRows
                    .Where(r => r.BangGiaId == bangGia.Id)
                    .Select(r => new ChiTietGiaLoaiCanHoResponse
                    {
                        Id = r.Id,
                        LoaiCanHoId = r.LoaiCanHoId,
                        LoaiCanHoTen = r.LoaiCanHoId != null ? LoaiCanHo.FromValue(r.LoaiCanHoId.Value)?.Name : null,
                        DonGia = r.DonGia
                    }).ToList();
                bangGia = bangGia with { GiaLoaiCanHos = details };
            }

            result.BangGia = bangGia;
        }

        return result;
    }

    public async Task<PagedResult<KhungGioDichVuResponse>> GetListKhungGioAsync(
        GetListKhungGioDichVuSpecification spec,
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
            { "IsActive", "kg.IsActive" },
            { "IsDeleted", "kg.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   kg.Id, kg.DichVuId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan, kg.IsActive
            FROM KhungGioDichVu kg
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<KhungGioDichVuReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new KhungGioDichVuResponse
        {
            Id = r.Id,
            DichVuId = r.DichVuId,
            GioBatDau = r.GioBatDau,
            GioKetThuc = r.GioKetThuc,
            TenKhungGio = r.TenKhungGio,
            NgayTrongTuan = r.NgayTrongTuan,
            IsActive = r.IsActive
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
        GetKhungGioDichVuByIdSpecification spec,
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

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sql = $"""
            SELECT kg.Id, kg.DichVuId, kg.GioBatDau, kg.GioKetThuc, kg.TenKhungGio, kg.NgayTrongTuan, kg.IsActive
            FROM KhungGioDichVu kg
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var row = await connection.QueryFirstOrDefaultAsync<KhungGioDichVuReadModel>(sql, parameters, transaction: transaction);
        if (row == null) return null;

        return new KhungGioDichVuResponse
        {
            Id = row.Id,
            DichVuId = row.DichVuId,
            GioBatDau = row.GioBatDau,
            GioKetThuc = row.GioKetThuc,
            TenKhungGio = row.TenKhungGio,
            NgayTrongTuan = row.NgayTrongTuan,
            IsActive = row.IsActive
        };
    }

    public async Task<PagedResult<BangGiaResponse>> GetListBangGiaAsync(
        GetListBangGiaSpecification spec,
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

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
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

        var rows = (await connection.QueryAsync<BangGiaReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new BangGiaResponse
        {
            Id = r.Id,
            DichVuId = r.DichVuId,
            TenBangGia = r.TenBangGia,
            NgayApDung = r.NgayApDung.DateTime,
            NgayKetThuc = r.NgayKetThuc?.DateTime,
            LoaiDinhGiaId = r.LoaiDinhGiaId,
            LoaiDinhGiaTen = LoaiDinhGia.FromValue(r.LoaiDinhGiaId)?.Name ?? string.Empty,
            LoaiDinhGiaCode = LoaiDinhGia.FromValue(r.LoaiDinhGiaId)?.Code ?? string.Empty,
            DonGia = r.DonGia,
            IsActive = r.IsActive
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
        GetBangGiaByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var bangGiaMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "bg.Id" },
            { "IsDeleted", "bg.IsDeleted" }
        };

        var luyTienMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TuMuc", "ct.TuMuc" }
        };

        var parameters = new DynamicParameters();
        var sqlWhereBg = DapperQueryBuilder.BuildWhere(spec, bangGiaMapping, parameters);

        var khungGioMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "KhungGioIsActive", "kg.IsActive" },
            { "KhungGioIsDeleted", "kg.IsDeleted" }
        };

        var sqlJoinsCtLuyTien = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ChiTietGiaLuyTien", "ct", "ct.BangGiaId = bg.Id")
        ], parameters);

        var sqlJoinsCtKhungGio = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("KhungGioDichVu", "kg", "kg.DichVuId = bg.DichVuId", Mapping: khungGioMapping),
            new JoinDefinition("ChiTietGiaKhungGio", "ct", "ct.BangGiaId = bg.Id AND ct.KhungGioId = kg.Id")
        ], parameters);

        var sqlJoinsCtLoaiCanHo = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ChiTietGiaLoaiCanHo", "ct", "ct.BangGiaId = bg.Id")
        ], parameters);

        var sqlOrderByLuyTien = DapperQueryBuilder.BuildOrderBy(spec, luyTienMapping, "TuMuc");
        var sql = $"""
            SELECT bg.Id, bg.TenBangGia, bg.NgayApDung, bg.NgayKetThuc, bg.LoaiDinhGiaId, bg.IsActive, bg.DonGia, bg.DichVuId
            FROM BangGia bg
            {sqlWhereBg};

            SELECT ct.Id, ct.TuMuc, ct.DenMuc, ct.DonGia, ct.BangGiaId
            FROM BangGia bg
            {sqlJoinsCtLuyTien}
            {sqlWhereBg}
            {sqlOrderByLuyTien};

            SELECT ct.Id, ct.KhungGioId, ct.DonGia, kg.TenKhungGio, ct.BangGiaId
            FROM BangGia bg
            {sqlJoinsCtKhungGio}
            {sqlWhereBg};

            SELECT ct.Id, ct.LoaiCanHoId, ct.DonGia, ct.BangGiaId
            FROM BangGia bg
            {sqlJoinsCtLoaiCanHo}
            {sqlWhereBg};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var row = await multi.ReadFirstOrDefaultAsync<BangGiaReadModel>();
        if (row == null) return null;

        var luyTienDetails = (await multi.ReadAsync<ChiTietGiaLuyTienReadModel>()).ToList();
        var khungGioDetails = (await multi.ReadAsync<ChiTietGiaKhungGioReadModel>()).ToList();
        var loaiCanHoRows = (await multi.ReadAsync<ChiTietGiaLoaiCanHoReadModel>()).ToList();

        var bangGia = new BangGiaResponse
        {
            Id = row.Id,
            DichVuId = row.DichVuId,
            TenBangGia = row.TenBangGia,
            NgayApDung = row.NgayApDung.DateTime,
            NgayKetThuc = row.NgayKetThuc?.DateTime,
            LoaiDinhGiaId = row.LoaiDinhGiaId,
            LoaiDinhGiaTen = LoaiDinhGia.FromValue(row.LoaiDinhGiaId)?.Name ?? string.Empty,
            LoaiDinhGiaCode = LoaiDinhGia.FromValue(row.LoaiDinhGiaId)?.Code ?? string.Empty,
            DonGia = row.DonGia,
            IsActive = row.IsActive
        };

        if (bangGia.LoaiDinhGiaId == LoaiDinhGia.LuyTien.Value)
        {
            bangGia = bangGia with
            {
                GiaLuyTiens = luyTienDetails.Select(x => new ChiTietGiaLuyTienResponse
                {
                    Id = x.Id,
                    TuMuc = x.TuMuc,
                    DenMuc = x.DenMuc,
                    DonGia = x.DonGia,
                    BangGiaId = x.BangGiaId
                }).ToList()
            };
        }
        else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio.Value)
        {
            bangGia = bangGia with
            {
                GiaKhungGios = khungGioDetails.Select(x => new ChiTietGiaKhungGioResponse
                {
                    Id = x.Id,
                    KhungGioId = x.KhungGioId,
                    DonGia = x.DonGia,
                    TenKhungGio = x.TenKhungGio,
                    BangGiaId = x.BangGiaId
                }).ToList()
            };
        }
        else if (bangGia.LoaiDinhGiaId == LoaiDinhGia.TheoDienTich.Value)
        {
            var details = loaiCanHoRows.Select(r => new ChiTietGiaLoaiCanHoResponse
            {
                Id = r.Id,
                LoaiCanHoId = r.LoaiCanHoId,
                LoaiCanHoTen = r.LoaiCanHoId != null ? LoaiCanHo.FromValue(r.LoaiCanHoId.Value)?.Name : null,
                DonGia = r.DonGia
            }).ToList();
            bangGia = bangGia with { GiaLoaiCanHos = details };
        }

        return bangGia;
    }

    public async Task<PagedResult<DangKyDichVuResponse>> GetListDangKyAsync(
        GetListDangKyDichVuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dk.Id" },
            { "NguoiDungId", "dk.CreatedBy" },
            { "DichVuId", "dk.DichVuId" },
            { "TrangThaiDangKyId", "dk.TrangThaiDangKyId" },
            { "TuNgay", "dk.NgayBatDau" },
            { "DenNgay", "dk.NgayBatDau" },
            { "SoLuong", "dk.SoLuong" },
            { "IsDeleted", "dk.IsDeleted" },
            { "NgayBatDau", "dk.NgayBatDau" },
            { "LoaiDichVuId", "dv.LoaiDichVuId" },
            { "MaDichVu", "dv.MaDichVu" },
            { "TenDichVu", "dv.TenDichVu" }
        };

        var parameters = new DynamicParameters();

        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sqlJoinsDv = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("DichVu", "dv", "dv.Id = dk.DichVuId", Type: JoinType.Inner)
        ], parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   dk.Id, dk.CanHoId, dk.DichVuId, dk.SoLuong, dk.NgayBatDau, dk.NgayKetThuc, dk.TrangThaiDangKyId,
                   dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId
            FROM DangKyDichVu dk
            {sqlJoinsDv}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var transaction = _dbContext.GetDbTransaction();

        var rows = (await connection.QueryAsync<DangKyDichVuReadModel>(sql, parameters, transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DangKyDichVuResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            DichVuId = r.DichVuId,
            MaDichVu = r.MaDichVu,
            TenDichVu = r.TenDichVu,
            LoaiDichVuId = r.LoaiDichVuId,
            LoaiDichVuTen = LoaiDichVu.FromValue(r.LoaiDichVuId)?.Name ?? string.Empty,
            SoLuong = r.SoLuong,
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            TrangThaiDangKyId = r.TrangThaiDangKyId,
            TrangThaiDangKyTen = TrangThaiDangKy.FromValue(r.TrangThaiDangKyId)?.Name ?? string.Empty
        }).ToList();

        return new PagedResult<DangKyDichVuResponse>
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
