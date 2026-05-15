using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class QuyThuChiQueryRepository : IQuyThuChiQueryRepository
{
    private readonly AppDbContext _dbContext;

    public QuyThuChiQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<QuyThuChiResponse>> GetNhatKyThuChiAsync(
        GetNhatKyThuChiSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "MaGiaoDich", "t.MaGiaoDich" },
            { "LoaiGiaoDichId", "t.LoaiGiaoDichId" },
            { "NgayGiaoDich", "t.NgayGiaoDich" },
            { "TongSoTien", "t.TongSoTien" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var chiTietMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CtDichVuId",    "ct.DichVuId" },
            { "CtNhomThongKe", "ct.NhomThongKe" }
        };

        // Chỉ JOIN ChiTietQuyThuChi khi spec có ít nhất 1 filter thuộc bảng con.
        // Nếu không có guard này, BuildJoin sẽ luôn render INNER JOIN → mất các QuyThuChi không có ChiTiet.
        var hasChiTietFilter = spec.Filters.Any(f => chiTietMapping.ContainsKey(f.PropertyName));
        var sqlJoin = hasChiTietFilter
            ? DapperQueryBuilder.BuildJoin(spec, new[]
            {
                new JoinDefinition(
                    Table: "ChiTietQuyThuChi",
                    Alias: "ct",
                    OnCondition: "ct.QuyThuChiId = t.Id",
                    Type: JoinType.Inner,
                    Mapping: chiTietMapping)
            }, parameters)
            : string.Empty;

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "t.NgayGiaoDich");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount,
                   t.Id, t.MaGiaoDich, t.LoaiGiaoDichId, t.TongSoTien, t.NgayGiaoDich,
                   t.PhuongThucThanhToanId, t.NguoiGiaoDich, t.ChungTuGoc
            FROM QuyThuChi t
            {sqlJoin}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;


        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<dynamic>(sql, parameters, transaction: transaction)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        // 1. Lấy IDs từ kết quả phân trang
        var ids = rows.Select(r => (int)r.Id).ToList();
        var detailsLookup = new Dictionary<int, List<ChiTietQuyThuChiResponse>>();

        // 2. Truy vấn chi tiết cho tất cả IDs nếu có
        if (ids.Any())
        {
            var sqlDetails = """
                SELECT Id, QuyThuChiId, SoTien, NhomThongKe, GhiChu, DichVuId
                FROM ChiTietQuyThuChi
                WHERE QuyThuChiId IN @Ids;
                """;
                
            var detailsRows = (await connection.QueryAsync<dynamic>(sqlDetails, new { Ids = ids }, transaction: transaction)).ToList();
            
            detailsLookup = detailsRows
                .Select(d => new 
                { 
                    QuyThuChiId = (int)d.QuyThuChiId,
                    Detail = new ChiTietQuyThuChiResponse
                    {
                        Id = (int)d.Id,
                        SoTien = (decimal)d.SoTien,
                        NhomThongKe = (string)(d.NhomThongKe ?? string.Empty),
                        GhiChu = (string?)d.GhiChu,
                        DichVuId = (int?)d.DichVuId
                    }
                })
                .GroupBy(x => x.QuyThuChiId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Detail).ToList());
        }

        var loaiMap = LoaiThuChi.ToDictionary();
        var ptThanhToanMap = PhuongThucThanhToan.ToDictionary();

        // 3. Map parent + children vào DTO (hỗ trợ init property)
        var items = rows.Select(r => {
            var id = (int)r.Id;
            return new QuyThuChiResponse
            {
                Id = id,
                MaGiaoDich = (string)r.MaGiaoDich,
                LoaiGiaoDichId = (int)r.LoaiGiaoDichId,
                TenLoaiGiaoDich = loaiMap.GetValueOrDefault((int)r.LoaiGiaoDichId, string.Empty),
                TongSoTien = (decimal)r.TongSoTien,
                NgayGiaoDich = (DateTimeOffset)r.NgayGiaoDich,
                PhuongThucThanhToanId = (int)r.PhuongThucThanhToanId,
                TenPhuongThucThanhToan = ptThanhToanMap.GetValueOrDefault((int)r.PhuongThucThanhToanId, string.Empty),
                NguoiGiaoDich = (string)r.NguoiGiaoDich,
                ChungTuGoc = (string?)r.ChungTuGoc,
                ChiTiets = detailsLookup.GetValueOrDefault(id, new())
            };
        }).ToList();

        return new PagedResult<QuyThuChiResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? (items.Count == 0 ? 20 : items.Count),
                TotalItems = totalCount
            }
        };
    }

    public async Task<QuyThuChiResponse?> GetByIdAsync(GetQuyThuChiByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "IsDeleted", "t.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sql = $"""
            SELECT t.Id, t.MaGiaoDich, t.LoaiGiaoDichId, t.TongSoTien, t.NgayGiaoDich,
                   t.PhuongThucThanhToanId, t.NguoiGiaoDich, t.ChungTuGoc
            FROM QuyThuChi t
            {sqlWhere};

            SELECT ct.Id, ct.SoTien, ct.NhomThongKe, ct.GhiChu, ct.DichVuId
            FROM ChiTietQuyThuChi ct
            INNER JOIN QuyThuChi t ON ct.QuyThuChiId = t.Id
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        
        var r = await multi.ReadFirstOrDefaultAsync<dynamic>();
        if (r == null) return null;

        var details = (await multi.ReadAsync<dynamic>()).ToList();

        var chiTiets = details.Select(d => new ChiTietQuyThuChiResponse
        {
            Id = (int)d.Id,
            SoTien = (decimal)d.SoTien,
            NhomThongKe = (string)(d.NhomThongKe ?? string.Empty),
            GhiChu = (string?)d.GhiChu,
            DichVuId = (int?)d.DichVuId
        }).ToList();

        return new QuyThuChiResponse
        {
            Id = (int)r.Id,
            MaGiaoDich = (string)r.MaGiaoDich,
            LoaiGiaoDichId = (int)r.LoaiGiaoDichId,
            TenLoaiGiaoDich = LoaiThuChi.ToDictionary().GetValueOrDefault((int)r.LoaiGiaoDichId, string.Empty),
            TongSoTien = (decimal)r.TongSoTien,
            NgayGiaoDich = (DateTimeOffset)r.NgayGiaoDich,
            PhuongThucThanhToanId = (int)r.PhuongThucThanhToanId,
            TenPhuongThucThanhToan = PhuongThucThanhToan.ToDictionary().GetValueOrDefault((int)r.PhuongThucThanhToanId, string.Empty),
            NguoiGiaoDich = (string)r.NguoiGiaoDich,
            ChungTuGoc = (string?)r.ChungTuGoc,
            ChiTiets = chiTiets
        };
    }

    public async Task<BaoCaoThuChiResponse> GetBaoCaoThuChiAsync(
        GetBaoCaoThuChiSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var transaction = _dbContext.GetDbTransaction();

        // 1. Calculate opening balance (Before spec.TuNgay)
        // Dùng DynamicParameters dùng chung cho cả 2 query — không dùng anonymous object.
        var baoCaoParameters = new DynamicParameters();
        baoCaoParameters.Add("TuNgay", spec.TuNgay);
        baoCaoParameters.Add("DenNgay", spec.DenNgay);

        var sqlOpening = """
            SELECT COALESCE(SUM(CASE WHEN LoaiGiaoDichId = 1 THEN TongSoTien ELSE -TongSoTien END), 0)
            FROM QuyThuChi
            WHERE NgayGiaoDich < @TuNgay AND IsDeleted = 0;
            """;
        var openingBalance = await connection.ExecuteScalarAsync<decimal>(sqlOpening, baoCaoParameters, transaction: transaction);

        // 2. Fetch all transactions details in period
        var sqlPeriod = """
            SELECT m.LoaiGiaoDichId, COALESCE(d.NhomThongKe, N'Khác') AS NhomThongKe, d.SoTien
            FROM QuyThuChi m
            INNER JOIN ChiTietQuyThuChi d ON m.Id = d.QuyThuChiId
            WHERE m.NgayGiaoDich >= @TuNgay AND m.NgayGiaoDich <= @DenNgay AND m.IsDeleted = 0;
            """;
        var rows = (await connection.QueryAsync<dynamic>(sqlPeriod, baoCaoParameters, transaction: transaction)).ToList();

        var totalInflow = rows.Where(r => (int)r.LoaiGiaoDichId == 1).Sum(r => (decimal)r.SoTien);
        var totalOutflow = rows.Where(r => (int)r.LoaiGiaoDichId == 2).Sum(r => (decimal)r.SoTien);

        // 3. Breakdown by category (NhomThongKe)
        var inflowBreakdown = rows
            .Where(r => (int)r.LoaiGiaoDichId == 1)
            .GroupBy(r => (string)r.NhomThongKe)
            .Select(g => new BaoCaoThuChiNhomResponse
            {
                NhomThongKe = g.Key,
                TongSoTien = g.Sum(r => (decimal)r.SoTien),
                SoGiaoDich = g.Count(),
                TyLePhanTram = totalInflow > 0 ? (double)(g.Sum(r => (decimal)r.SoTien) / totalInflow) * 100 : 0
            })
            .OrderByDescending(x => x.TongSoTien)
            .ToList();

        var outflowBreakdown = rows
            .Where(r => (int)r.LoaiGiaoDichId == 2)
            .GroupBy(r => (string)r.NhomThongKe)
            .Select(g => new BaoCaoThuChiNhomResponse
            {
                NhomThongKe = g.Key,
                TongSoTien = g.Sum(r => (decimal)r.SoTien),
                SoGiaoDich = g.Count(),
                TyLePhanTram = totalOutflow > 0 ? (double)(g.Sum(r => (decimal)r.SoTien) / totalOutflow) * 100 : 0
            })
            .OrderByDescending(x => x.TongSoTien)
            .ToList();

        return new BaoCaoThuChiResponse
        {
            TuNgay = spec.TuNgay,
            DenNgay = spec.DenNgay,
            SoDuDauKy = openingBalance,
            TongThu = totalInflow,
            TongChi = totalOutflow,
            DongTienThuan = totalInflow - totalOutflow,
            SoDuCuoiKy = openingBalance + totalInflow - totalOutflow,
            DanhSachKhoanThu = inflowBreakdown,
            DanhSachKhoanChi = outflowBreakdown
        };
    }

    public async Task<List<BaoCaoCongNoCanHoResponse>> GetBaoCaoCongNoCanHoAsync(
        GetBaoCaoCongNoCanHoSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        // Đọc tham số từ spec properties — không add thủ công ngoài spec.
        var parameters = new DynamicParameters();
        parameters.Add("ToaNhaId", spec.ToaNhaId);
        parameters.Add("Thang", spec.Thang);
        parameters.Add("Nam", spec.Nam);

        var transaction = _dbContext.GetDbTransaction();

        // Fetch all active apartments with optional filters
        var sqlApartments = """
            SELECT c.Id AS CanHoId, c.MaCanHo, t.TenTang AS TangName, tn.TenToaNha AS ToaNhaName,
                   COALESCE(nd.Ho + ' ' + nd.Ten, N'Chưa có chủ hộ') AS ChuHoName
            FROM CanHo c
            INNER JOIN Tang t ON c.TangId = t.Id
            INNER JOIN ToaNha tn ON t.ToaNhaId = tn.Id
            LEFT JOIN QuanHeCuTru qh ON qh.CanHoId = c.Id AND qh.IsDeleted = 0 AND qh.TrangThaiCuTruId = 1 AND qh.LoaiQuanHeCuTruId = 1
            LEFT JOIN NguoiDung nd ON qh.NguoiDungId = nd.Id
            WHERE c.IsDeleted = 0
              AND (@ToaNhaId IS NULL OR tn.Id = @ToaNhaId)
            ORDER BY tn.TenToaNha, t.Id, c.MaCanHo;
            """;

        var apartments = (await connection.QueryAsync<dynamic>(sqlApartments, parameters, transaction: transaction)).ToList();

        if (!apartments.Any()) return [];

        // Fetch all active invoices (excluding DaHuy = 8) — dùng chung parameters.
        var sqlInvoices = """
            SELECT hd.Id AS HoaDonId, hd.CanHoId, hd.TongTien, hd.Thang, hd.Nam,
                   COALESCE((
                       SELECT SUM(gd.SoTien)
                       FROM GiaoDichThanhToan gd
                       INNER JOIN ChiTietHoaDon ct ON gd.ChiTietHoaDonId = ct.Id
                       WHERE ct.HoaDonId = hd.Id AND gd.IsDeleted = 0
                   ), 0) AS TotalPaid,
                   COALESCE((
                       SELECT SUM(gd.SoTien)
                       FROM GiaoDichThanhToan gd
                       INNER JOIN ChiTietHoaDon ct ON gd.ChiTietHoaDonId = ct.Id
                       WHERE ct.HoaDonId = hd.Id AND gd.IsDeleted = 0
                         AND MONTH(gd.NgayGiaoDich) = @Thang AND YEAR(gd.NgayGiaoDich) = @Nam
                   ), 0) AS PaidInPeriod
            FROM HoaDon hd
            WHERE hd.IsDeleted = 0 AND hd.TrangThaiHoaDonId <> 8;
            """;

        var invoices = (await connection.QueryAsync<dynamic>(sqlInvoices, parameters, transaction: transaction)).ToList();

        var invoiceGroups = invoices.GroupBy(i => (int)i.CanHoId).ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<BaoCaoCongNoCanHoResponse>();

        foreach (var apt in apartments)
        {
            int canHoId = (int)apt.CanHoId;
            decimal noDauKy = 0m;
            decimal phatSinhTrongKy = 0m;
            decimal daThanhToanTrongKy = 0m;

            if (invoiceGroups.TryGetValue(canHoId, out var aptInvoices))
            {
                foreach (var inv in aptInvoices)
                {
                    int invThang = (int)inv.Thang;
                    int invNam = (int)inv.Nam;
                    decimal tongTien = (decimal)inv.TongTien;
                    decimal totalPaid = (decimal)inv.TotalPaid;
                    decimal paidInPeriod = (decimal)inv.PaidInPeriod;

                    // A. Invoice is prior to reporting month
                    if (invNam < spec.Nam || (invNam == spec.Nam && invThang < spec.Thang))
                    {
                        // Debt at the beginning of current period is: TongTien - Paid prior to this month.
                        // Paid prior to this month = TotalPaid - Paid in this month (paidInPeriod).
                        decimal paidBeforePeriod = totalPaid - paidInPeriod;
                        decimal remainingAtStart = tongTien - paidBeforePeriod;
                        if (remainingAtStart > 0)
                        {
                            noDauKy += remainingAtStart;
                        }
                    }
                    // B. Invoice belongs to reporting month
                    else if (invNam == spec.Nam && invThang == spec.Thang)
                    {
                        phatSinhTrongKy += tongTien;
                    }

                    // Accumulate all payments made for this apartment's invoices *during the current month*
                    daThanhToanTrongKy += paidInPeriod;
                }
            }

            decimal noCuoiKy = noDauKy + phatSinhTrongKy - daThanhToanTrongKy;
            if (noCuoiKy < 0) noCuoiKy = 0m; // Prevent negative debt representation due to pre-payments or roundings

            result.Add(new BaoCaoCongNoCanHoResponse
            {
                CanHoId = canHoId,
                MaCanHo = (string)apt.MaCanHo,
                TenToaNha = (string)apt.ToaNhaName,
                TenTang = (string)apt.TangName,
                TenChuHo = (string)apt.ChuHoName,
                NoDauKy = noDauKy,
                PhatSinhTrongKy = phatSinhTrongKy,
                DaThanhToanTrongKy = daThanhToanTrongKy,
                NoCuoiKy = noCuoiKy
            });
        }

        return result;
    }

    public async Task<List<BaoCaoCongNoToaNhaResponse>> GetBaoCaoCongNoToaNhaAsync(
        GetBaoCaoCongNoToaNhaSpecification spec,
        CancellationToken cancellationToken = default)
    {
        // Tái sử dụng GetBaoCaoCongNoCanHoAsync với spec riêng cho toàn bộ tòa nhà (ToaNhaId = null).
        // Không tạo Spec mới trong Infrastructure — thay vào đó truyền giá trị cần thiết trực tiếp vào method.
        var allAptSpec = new GetBaoCaoCongNoCanHoSpecification(toaNhaId: null, thang: spec.Thang, nam: spec.Nam);
        var aptDebts = await GetBaoCaoCongNoCanHoAsync(allAptSpec, cancellationToken);

        if (!aptDebts.Any()) return [];

        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var sqlToaNha = "SELECT Id, TenToaNha FROM ToaNha WHERE IsDeleted = 0 ORDER BY TenToaNha;";
        var buildings = (await connection.QueryAsync<dynamic>(sqlToaNha, transaction: _dbContext.GetDbTransaction())).ToList();

        var result = new List<BaoCaoCongNoToaNhaResponse>();

        foreach (var b in buildings)
        {
            int toaNhaId = (int)b.Id;
            string tenToaNha = (string)b.TenToaNha;

            var bApts = aptDebts.Where(x => x.TenToaNha == tenToaNha).ToList();

            if (!bApts.Any()) continue;

            int totalApts = bApts.Count;
            int debtorApts = bApts.Count(x => x.NoCuoiKy > 0);
            decimal noDauKy = bApts.Sum(x => x.NoDauKy);
            decimal phatSinh = bApts.Sum(x => x.PhatSinhTrongKy);
            decimal daThu = bApts.Sum(x => x.DaThanhToanTrongKy);
            decimal conLai = bApts.Sum(x => x.NoCuoiKy);

            double tyLeThuHoi = phatSinh > 0 ? (double)(daThu / phatSinh) * 100 : 0;
            if (tyLeThuHoi > 100) tyLeThuHoi = 100; // Cap at 100%

            result.Add(new BaoCaoCongNoToaNhaResponse
            {
                ToaNhaId = toaNhaId,
                TenToaNha = tenToaNha,
                TongSoCanHo = totalApts,
                SoCanHoNoPhi = debtorApts,
                TongNoDauKy = noDauKy,
                TongPhatSinh = phatSinh,
                TongDaThu = daThu,
                TongNoConLai = conLai,
                TyLeThuHoi = Math.Round(tyLeThuHoi, 2)
            });
        }

        return result;
    }
}
