using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class HoaDonQueryRepository : IHoaDonQueryRepository
{
    private readonly AppDbContext _dbContext;

    public HoaDonQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<HoaDonResponse>> GetListAsync(
        GetListHoaDonSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hd.Id" },
            { "MaHoaDon", "hd.MaHoaDon" },
            { "CanHoId", "hd.CanHoId" },
            { "DotThanhToanId", "hd.DotThanhToanId" },
            { "Thang", "hd.Thang" },
            { "Nam", "hd.Nam" },
            { "TrangThaiHoaDonId", "hd.TrangThaiHoaDonId" },
            { "NgayLap", "hd.NgayLap" },
            { "NgayHanThanhToan", "hd.NgayHanThanhToan" },
            { "TongTien", "hd.TongTien" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   hd.Id, hd.CanHoId, hd.DotThanhToanId, hd.MaHoaDon, hd.Thang, hd.Nam, 
                   hd.NgayLap, hd.NgayHanThanhToan, hd.TongTien, hd.TrangThaiHoaDonId
            FROM HoaDon hd
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<HoaDonReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new HoaDonResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            MaHoaDon = r.MaHoaDon,
            Thang = r.Thang,
            Nam = r.Nam,
            NgayLap = r.NgayLap,
            NgayHanThanhToan = r.NgayHanThanhToan,
            TongTien = r.TongTien,
            TrangThaiHoaDonId = r.TrangThaiHoaDonId,
            TrangThaiHoaDonTen = TrangThaiHoaDon.FromValue(r.TrangThaiHoaDonId)?.Name ?? string.Empty
        }).ToList();

        return new PagedResult<HoaDonResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? (items.Count > 0 ? items.Count : 10),
                TotalItems = totalCount
            }
        };
    }

    public async Task<HoaDonDetailResponse?> GetByIdAsync(
        GetHoaDonByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hd.Id" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoinsCt = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ChiTietHoaDon", "ct", "ct.HoaDonId = hd.Id", Type: JoinType.Inner)
        ], parameters);

        var sql = $"""
            SELECT hd.Id, hd.CanHoId, hd.DotThanhToanId, hd.MaHoaDon, hd.Thang, hd.Nam, 
                   hd.NgayLap, hd.NgayHanThanhToan, hd.TongTien, hd.TrangThaiHoaDonId, hd.GhiChu
            FROM HoaDon hd
            {sqlWhere};

            SELECT ct.Id, ct.HoaDonId, ct.LoaiChiTietHoaDonId, ct.TenMucPhi, 
                   ct.SoLuong, ct.DonGia, ct.ThanhTien, ct.GhiChu,
                   bg.LoaiDinhGiaId
            FROM HoaDon hd
            INNER JOIN ChiTietHoaDon ct ON ct.HoaDonId = hd.Id
            LEFT JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var hoaDonRow = await multi.ReadFirstOrDefaultAsync<HoaDonReadModel>();
        if (hoaDonRow == null) return null;

        var chiTietRows = (await multi.ReadAsync<ChiTietHoaDonReadModel>()).ToList();

        var response = new HoaDonDetailResponse
        {
            Id = hoaDonRow.Id,
            CanHoId = hoaDonRow.CanHoId,
            MaHoaDon = hoaDonRow.MaHoaDon,
            Thang = hoaDonRow.Thang,
            Nam = hoaDonRow.Nam,
            NgayLap = hoaDonRow.NgayLap,
            NgayHanThanhToan = hoaDonRow.NgayHanThanhToan,
            TongTien = hoaDonRow.TongTien,
            TrangThaiHoaDonId = hoaDonRow.TrangThaiHoaDonId,
            TrangThaiHoaDonTen = TrangThaiHoaDon.FromValue(hoaDonRow.TrangThaiHoaDonId)?.Name ?? string.Empty,
            GhiChu = hoaDonRow.GhiChu,
            ChiTietHoaDons = chiTietRows.Select(ct => new ChiTietHoaDonResponse
            {
                Id = ct.Id,
                LoaiChiTietHoaDonId = ct.LoaiChiTietHoaDonId,
                LoaiChiTietHoaDonTen = LoaiChiTietHoaDon.FromValue(ct.LoaiChiTietHoaDonId)?.Name ?? string.Empty,
                TenMucPhi = ct.TenMucPhi,
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                ThanhTien = ct.ThanhTien,
                LoaiDinhGiaId = ct.LoaiDinhGiaId,
                LoaiDinhGiaTen = ct.LoaiDinhGiaId.HasValue ? LoaiDinhGia.FromValue(ct.LoaiDinhGiaId.Value)?.Name : string.Empty,
                GhiChu = ct.GhiChu
            }).ToList()
        };

        return response;
    }

    public async Task<ChiTietCoDinhResponse?> GetChiTietCoDinhAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        var sql = """
            SELECT ct.Id, ct.TenMucPhi, ct.SoLuong, ct.DonGia, ct.ThanhTien, ct.GhiChu
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            WHERE ct.Id = @Id AND bg.LoaiDinhGiaId = 1
            """;

        return await connection.QueryFirstOrDefaultAsync<ChiTietCoDinhResponse>(sql, new { Id = chiTietHoaDonId }, transaction: _dbContext.GetDbTransaction());
    }

    public async Task<ChiTietLuyTienResponse?> GetChiTietLuyTienAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        var sql = """
            SELECT ct.Id, ct.TenMucPhi, ct.ChiSoCu, ct.ChiSoMoi, ct.ThanhTien,
                   (ct.ChiSoMoi - ct.ChiSoCu) AS SoLuongTieuThu,
                   tl.FileUrl AS AnhDongHoUrl
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            LEFT JOIN ChiSoTieuThu cs ON cs.HoaDonId = hd.Id AND cs.DichVuId = ct.DichVuId
            LEFT JOIN TepTaiLieu tl ON cs.AnhDongHoId = tl.Id
            WHERE ct.Id = @Id AND bg.LoaiDinhGiaId = 2;

            SELECT bglt.TuMuc, bglt.DenMuc, bglt.DonGia
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            INNER JOIN ChiTietGiaLuyTien bglt ON bglt.BangGiaId = bg.Id
            WHERE ct.Id = @Id
            ORDER BY bglt.TuMuc;
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = chiTietHoaDonId }, transaction: _dbContext.GetDbTransaction());

        var response = await multi.ReadFirstOrDefaultAsync<ChiTietLuyTienResponse>();
        if (response == null) return null;

        var tiers = (await multi.ReadAsync<dynamic>()).ToList();
        var consumption = response.SoLuongTieuThu;

        int index = 1;
        foreach (var tier in tiers)
        {
            if (consumption <= tier.TuMuc) break;

            var tu = (decimal)tier.TuMuc;
            var den = (decimal?)tier.DenMuc;
            var donGia = (decimal)tier.DonGia;

            var amountInTier = (den.HasValue ? Math.Min(consumption, den.Value) : consumption) - tu;

            response.BacThang.Add(new ChiTietGiaLuyTienItemResponse
            {
                TenBac = $"Bậc {index++}",
                TuSo = tu,
                DenSo = den,
                SoLuong = amountInTier,
                DonGia = donGia,
                ThanhTien = amountInTier * donGia
            });

            if (den.HasValue && consumption <= den.Value) break;
        }

        return response;
    }

    public async Task<ChiTietDienTichResponse?> GetChiTietDienTichAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        var sql = """
            SELECT ct.Id, ct.TenMucPhi, ct.SoLuong AS DienTich, ct.DonGia, ct.ThanhTien,
                   ch.LoaiCanHoId
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN CanHo ch ON ch.Id = hd.CanHoId
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            WHERE ct.Id = @Id AND bg.LoaiDinhGiaId = 6
            """;

        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = chiTietHoaDonId }, transaction: _dbContext.GetDbTransaction());

        if (row == null) return null;

        return new ChiTietDienTichResponse
        {
            Id = row.Id,
            TenMucPhi = row.TenMucPhi,
            DienTich = row.DienTich,
            DonGia = row.DonGia,
            ThanhTien = row.ThanhTien,
            TenLoaiCanHo = LoaiCanHo.FromValue((int)row.LoaiCanHoId)?.Name ?? string.Empty
        };
    }

    public async Task<ChiTietKhungGioResponse?> GetChiTietKhungGioAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        var sql = """
            SELECT ct.Id, ct.TenMucPhi, ct.ThanhTien
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            WHERE ct.Id = @Id AND bg.LoaiDinhGiaId = 7;

            SELECT bgkg.DonGia, kg.TenKhungGio, kg.GioBatDau, kg.GioKetThuc
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            INNER JOIN BangGia bg ON bg.DichVuId = ct.DichVuId 
                AND hd.NgayLap >= bg.NgayApDung 
                AND (bg.NgayKetThuc IS NULL OR hd.NgayLap <= bg.NgayKetThuc)
            INNER JOIN ChiTietGiaKhungGio bgkg ON bgkg.BangGiaId = bg.Id
            INNER JOIN KhungGioDichVu kg ON kg.Id = bgkg.KhungGioId
            WHERE ct.Id = @Id;
            """;

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = chiTietHoaDonId }, transaction: _dbContext.GetDbTransaction());

        var response = await multi.ReadFirstOrDefaultAsync<ChiTietKhungGioResponse>();
        if (response == null) return null;

        var slots = (await multi.ReadAsync<dynamic>()).ToList();
        foreach (var slot in slots)
        {
            response.KhungGios.Add(new ChiTietGiaKhungGioItemResponse
            {
                TenKhungGio = slot.TenKhungGio,
                GioBatDau = ((TimeSpan)slot.GioBatDau).ToString(@"hh\:mm"),
                GioKetThuc = ((TimeSpan)slot.GioKetThuc).ToString(@"hh\:mm"),
                DonGia = slot.DonGia
            });
        }

        return response;
    }

    public async Task<(string TenMucPhi, int LoaiChiTietHoaDonId, string? ResidentName, int? DichVuId)> GetChiTietHoaDonInfoAsync(int chiTietHoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        var sql = """
            SELECT ct.TenMucPhi, ct.LoaiChiTietHoaDonId, ct.DichVuId, nd.Ho + ' ' + nd.Ten AS ResidentName
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON ct.HoaDonId = hd.Id
            LEFT JOIN QuanHeCuTru qh ON qh.CanHoId = hd.CanHoId AND qh.TrangThaiCuTruId = 1 AND qh.LoaiQuanHeCuTruId = 1
            LEFT JOIN NguoiDung nd ON nd.Id = qh.NguoiDungId
            WHERE ct.Id = @Id
            """;

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = chiTietHoaDonId }, transaction: _dbContext.GetDbTransaction());

        if (result == null) return ("UNKNOWN", 0, null, null);

        return ((string)result.TenMucPhi, (int)result.LoaiChiTietHoaDonId, (string?)result.ResidentName, (int?)result.DichVuId);
    }
}
