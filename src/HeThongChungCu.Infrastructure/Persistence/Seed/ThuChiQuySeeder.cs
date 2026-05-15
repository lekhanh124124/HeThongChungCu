using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class QuyThuChiSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding QuyThuChi financial transaction logs...");

        // ── Guard: skip sớm nếu đã có dữ liệu QuyThuChi ────────────────────────
        // QUAN TRỌNG: phải check TRƯỚC khi seed HoaDonDoiTac.
        // UpdateStatus(DaThanhToan) raise domain event → SaveChanges tự tạo QuyThu record.
        // Nếu guard check SAU đó, sẽ thấy 2 record và skip toàn bộ phần seed thủ công.
        if (await context.QuyThuChis.AnyAsync())
        {
            logger.LogInformation("QuyThuChi already seeded. Skipping.");
            return;
        }

        // ── Partner Invoices (HoaDonDoiTac) ─────────────────────────────────────
        var schindlerContract = await context.HopDongDoiTacs
            .FirstOrDefaultAsync(h => h.SoHopDong == "HD-SCH-2026");

        if (schindlerContract != null && !await context.HoaDonDoiTacs.AnyAsync())
        {
            logger.LogInformation("Seeding HoaDonDoiTac partner invoices...");

            var invoices = new List<HoaDonDoiTac>
            {
                new HoaDonDoiTac(schindlerContract.Id, 3, 2026, 15_000_000m, null, "Bảo trì thang máy T3/2026"),
                new HoaDonDoiTac(schindlerContract.Id, 4, 2026, 15_000_000m, null, "Bảo trì thang máy T4/2026"),
                new HoaDonDoiTac(schindlerContract.Id, 5, 2026, 15_000_000m, null, "Bảo trì thang máy T5/2026"),
            };
            invoices[0].UpdateStatus(TrangThaiThanhToanDoiTac.DaThanhToan);
            invoices[1].UpdateStatus(TrangThaiThanhToanDoiTac.DaThanhToan);

            await context.HoaDonDoiTacs.AddRangeAsync(invoices);
            DatabaseSeeder.ClearAllDomainEvents(context);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded 3 partner invoices.");
        }

        // ── Build QuyThuChi records ────────────────────────────────────────────

        // Lấy các ID thực tế từ database để seed cho phong phú
        // Dùng StringComparer.OrdinalIgnoreCase để tránh lỗi typo hoa/thường
        var dichVus = await context.DichVus.ToDictionaryAsync(d => d.MaDichVu, d => (int?)d.Id, StringComparer.OrdinalIgnoreCase);

        // Helper để lấy ID an toàn
        int? GetDv(string code)
        {
            if (dichVus.TryGetValue(code, out var id)) return id;
            logger.LogWarning($"[QuyThuChiSeeder] Service code '{code}' not found in database. DichVuId will be null.");
            return null;
        }

        var dvManagement = GetDv("MANAGEMENT_FEE");
        var dvElectric = GetDv("ELECTRICITY");
        var dvWater = GetDv("WATER");
        var dvInternet = GetDv("INTERNET_BASIC");
        var dvMotor = GetDv("PK_MOTOR");
        var dvCar = GetDv("PK_CAR");
        var dvWashBike = GetDv("DV_WASH_BIKE");
        var dvTennis = GetDv("DV_TENNIS");

        var thiCongId = await context.YeuCauThiCongs.Select(d => (int?)d.Id).FirstOrDefaultAsync();
        var suaChuaId = await context.YeuCauSuaChuas.Select(d => (int?)d.Id).FirstOrDefaultAsync();

        var list = new List<QuyThuChi>();
        var gio7 = TimeSpan.FromHours(7);

        // ════════════════════════════════════════════════════════════
        // THÁNG 1 & 2 / 2026 — tạo số dư đầu kỳ (trước TuNgay kỳ báo cáo)
        // Mục đích: kiểm tra SoDuDauKy trong BaoCaoThuChi
        // ════════════════════════════════════════════════════════════

        // T1: Thu phí quản lý (multi-line chiTiet — 2 nhóm)
        list.Add(CreateThu("THU-20260110-001",
            new DateTimeOffset(2026, 1, 10, 8, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Cư dân Block A",
            "BIENlai-T1A",
            chiTiets:
            [
                (dvManagement, 80_000_000m, "Thu phí quản lý vận hành", "Block A - T1/2026"),
                (dvCar,        30_000_000m, "Thu phí gửi xe",           "Xe ô tô Block A - T1/2026"),
            ]));

        // T1: Chi vận hành (multi-line chiTiet)
        list.Add(CreateChi("CHI-20260125-001",
            new DateTimeOffset(2026, 1, 25, 9, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Công ty Điện lực EVN",
            "HD-EVN-T1-2026",
            chiTiets:
            [
                ("Điện tổng tòa nhà",      60_000_000m, "Hóa đơn điện tổng T1/2026", dvElectric),
                ("Điện sinh hoạt cư dân",  25_000_000m, "Thu hộ tiền điện T1/2026", dvElectric),
            ]));

        // T2: Thu phí gửi xe + tiện ích
        list.Add(CreateThu("THU-20260205-001",
            new DateTimeOffset(2026, 2, 5, 10, 0, 0, gio7),
            PhuongThucThanhToan.TienMat,
            "Cư dân tòa nhà",
            "BIENHAI-T2",
            chiTiets:
            [
                (dvManagement, 55_000_000m, "Thu phí quản lý vận hành", "Block B - T2/2026"),
                (GetDv("DV_POOL"), 8_500_000m, "Thu phí sử dụng tiện ích", "BBQ, hồ bơi T2/2026"),
            ]));

        // T2: Chi bảo trì thang máy
        list.Add(CreateChi("CHI-20260220-001",
            new DateTimeOffset(2026, 2, 20, 14, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Schindler Việt Nam",
            "HD-SCH-T2-2026",
            chiTiets:
            [
                ("Chi trả nhà cung cấp/đối tác", 15_000_000m, "Phí bảo trì thang máy T2/2026", null),
            ]));

        // ════════════════════════════════════════════════════════════
        // THÁNG 3 / 2026 — dữ liệu chính kỳ báo cáo
        // Mục đích: test BaoCaoThuChi, lọc NhomThongKe, lọc DichVuId
        // ════════════════════════════════════════════════════════════

        // THÔNG BÁO: Kể từ tháng 3/2026, các khoản Thu phí (Quản lý, Điện, Nước, Xe) 
        // sẽ được HoaDonSeeder tự động hạch toán vào Quỹ thông qua Domain Event.
        // ThuChiQuySeeder chỉ seed các khoản Thu/Chi đặc thù không qua hóa đơn cư dân.

        // (Thu phí gửi xe T3 đã được HoaDonSeeder tự động hạch toán)

        // Thu đặt cọc thi công (test NhomThongKe = "Thu đặt cọc thi công")
        list.Add(CreateThu("THU-20260310-001",
            new DateTimeOffset(2026, 3, 10, 9, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Cư dân Trần Văn Minh - A-05.12",
            "UNC-THC-2026-001",
            chiTiets:
            [
                ((int?)null, 20_000_000m, "Thu đặt cọc thi công", "Ký quỹ sửa nội thất căn A-05.12"),
            ]));

        // Thu phí sử dụng tiện ích + Thu khác (multi-nhóm, ví điện tử)
        list.Add(CreateThu("THU-20260315-001",
            new DateTimeOffset(2026, 3, 15, 11, 0, 0, gio7),
            PhuongThucThanhToan.ViDienTu,
            "Nhiều cư dân",
            null,
            chiTiets:
            [
                (GetDv("DV_BBQ"),  12_000_000m, "Thu phí sử dụng tiện ích", "Sảnh BBQ + hồ bơi T3/2026"),
                ((int?)null,        5_000_000m, "Thu khác",                  "Quảng cáo thang máy tháng 3"),
            ]));

        // Thu phí dịch vụ sửa chữa (test NhomThongKe, keyword)
        list.Add(CreateThu("THU-20260318-001",
            new DateTimeOffset(2026, 3, 18, 14, 0, 0, gio7),
            PhuongThucThanhToan.TienMat,
            "Lê Thị Hoa - B-03.07",
            "BIENRAI-SC-001",
            chiTiets:
            [
                (GetDv("DV_YC_SUACHUA"), 800_000m, "Thu phí dịch vụ sửa chữa", "Sửa vòi nước, thay bóng đèn"),
            ]));

        // Chi vận hành (multi-line — điện + nước + lương)
        list.Add(CreateChi("CHI-20260320-001",
            new DateTimeOffset(2026, 3, 20, 10, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Công ty Điện lực EVN",
            "HD-EVN-T3-2026",
            chiTiets:
            [
                ("Chi vận hành", 98_600_000m, "Hóa đơn điện tổng + sinh hoạt T3/2026", dvElectric),
            ]));

        // Chi bảo trì thang máy Schindler
        list.Add(CreateChi("CHI-20260325-001",
            new DateTimeOffset(2026, 3, 25, 9, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Schindler Việt Nam",
            "HD-SCH-T3-2026",
            chiTiets:
            [
                ("Chi trả nhà cung cấp/đối tác", 15_000_000m, "Phí bảo trì thang máy T3/2026", null),
            ]));

        // Chi văn phòng phẩm + in ấn (tiền mặt, multi-line — test NhomThongKe)
        list.Add(CreateChi("CHI-20260328-001",
            new DateTimeOffset(2026, 3, 28, 8, 30, 0, gio7),
            PhuongThucThanhToan.TienMat,
            "Nhà sách Nguyễn Huệ",
            "HD-NSH-9921",
            chiTiets:
            [
                ("Văn phòng phẩm", 450_000m,  "Bút, giấy A4, ghim kẹp cho BQL", null),
                ("Chi phí in ấn",  180_000m,  "In thông báo cư dân tháng 3",    null),
            ]));

        // Chi hoàn cọc thi công (test NhomThongKe = "Chi hoàn cọc thi công")
        list.Add(CreateChi("CHI-20260329-001",
            new DateTimeOffset(2026, 3, 29, 14, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Nguyễn Văn An - A-02.03",
            "UNC-HOAN-COC-001",
            chiTiets:
            [
                ("Chi hoàn cọc thi công", 20_000_000m, "Hoàn cọc sửa nội thất căn A-02.03 đã nghiệm thu", null),
            ]));

        // ════════════════════════════════════════════════════════════
        // THÁNG 4 / 2026
        // Mục đích: test phân trang (nhiều bản ghi), filter theo tháng
        // ════════════════════════════════════════════════════════════

        // (Thu phí T4 đã được HoaDonSeeder tự động hạch toán)

        list.Add(CreateThu("THU-20260410-001",
            new DateTimeOffset(2026, 4, 10, 11, 0, 0, gio7),
            PhuongThucThanhToan.ViDienTu,
            "Cư dân đặt tiện ích",
            null,
            chiTiets:
            [
                (GetDv("DV_COMMUNITY"), 9_500_000m, "Thu phí sử dụng tiện ích", "Phòng SHCD, BBQ T4/2026"),
            ]));

        list.Add(CreateChi("CHI-20260420-001",
            new DateTimeOffset(2026, 4, 20, 10, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Công ty Điện lực EVN",
            "HD-EVN-T4-2026",
            chiTiets:
            [
                ("Chi vận hành",               95_000_000m, "Điện tổng + sinh hoạt T4/2026", dvElectric),
                ("Chi bảo trì, sửa chữa hạ tầng", 8_500_000m, "Thay bóng đèn hành lang, sửa camera", null),
            ]));

        list.Add(CreateChi("CHI-20260425-001",
            new DateTimeOffset(2026, 4, 25, 9, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Schindler Việt Nam",
            "HD-SCH-T4-2026",
            chiTiets:
            [
                ("Chi trả nhà cung cấp/đối tác", 15_000_000m, "Phí bảo trì thang máy T4/2026", null),
            ]));

        // ════════════════════════════════════════════════════════════
        // THÁNG 5 / 2026 (tháng hiện tại)
        // Mục đích: test báo cáo kỳ hiện tại, công nợ tháng 5
        // ════════════════════════════════════════════════════════════

        // (Thu phí T5 đã được HoaDonSeeder tự động hạch toán một phần qua GiaoDichThanhToan)

        // Thu đặt cọc thi công mới trong tháng 5
        list.Add(CreateThu("THU-20260508-001",
            new DateTimeOffset(2026, 5, 8, 9, 30, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Phạm Thị Lan - C-07.05",
            "UNC-THC-2026-002",
            chiTiets:
            [
                ((int?)null, 20_000_000m, "Thu đặt cọc thi công", "Ký quỹ sửa nội thất căn C-07.05"),
            ]));

        // Thu khác (quảng cáo sảnh)
        list.Add(CreateThu("THU-20260512-001",
            new DateTimeOffset(2026, 5, 12, 14, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Công ty Quảng cáo Kính Vạn Hoa",
            "HD-QC-KVH-001",
            chiTiets:
            [
                ((int?)null, 6_000_000m, "Thu khác", "Quảng cáo màn hình thang máy T5/2026"),
            ]));

        // Chi vận hành tháng 5 (multi-line phức tạp)
        list.Add(CreateChi("CHI-20260514-001",
            new DateTimeOffset(2026, 5, 14, 10, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Công ty Điện lực EVN",
            "HD-EVN-T5-2026",
            chiTiets:
            [
                ("Chi vận hành",               102_000_000m, "Điện tổng tòa nhà + sinh hoạt T5/2026", dvElectric),
                ("Chi trả đối tác viễn thông",  18_000_000m, "Thanh toán hạ tầng Viettel T5",        dvInternet),
                ("Chi dịch vụ rửa xe",           4_200_000m, "Thanh toán đối tác WashUp T5",         dvWashBike),
                ("Chi bảo trì, sửa chữa hạ tầng",  5_200_000m, "Sửa hệ thống camera an ninh B2", null),
            ]));

        // Chi bảo trì tháng 5 - Schindler
        list.Add(CreateChi("CHI-20260514-002",
            new DateTimeOffset(2026, 5, 14, 11, 0, 0, gio7),
            PhuongThucThanhToan.ChuyenKhoan,
            "Schindler Việt Nam",
            "HD-SCH-T5-2026",
            chiTiets:
            [
                ("Chi trả nhà cung cấp/đối tác", 15_000_000m, "Bảo trì thang máy T5/2026", null),
            ]));

        // ════════════════════════════════════════════════════════════
        await context.QuyThuChis.AddRangeAsync(list);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {list.Count} QuyThuChi records covering T1–T5/2026.");
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static QuyThuChi CreateThu(
        string maGiaoDich,
        DateTimeOffset ngay,
        PhuongThucThanhToan phuongThuc,
        string nguoiGiaoDich,
        string? chungTu,
        IEnumerable<(int? dichVuId, decimal soTien, string nhomThongKe, string? ghiChu)> chiTiets)
    {
        var result = QuyThuChi.CreateThu(maGiaoDich, ngay, phuongThuc, nguoiGiaoDich, chungTu);
        if (result.IsFailure) throw new Exception($"Seed QuyThu failed [{maGiaoDich}]: {result.Errors.First().Description}");

        var t = result.Value;
        foreach (var (dichVuId, soTien, nhomThongKe, ghiChu) in chiTiets)
            t.AddChiTiet(soTien, nhomThongKe, ghiChu, dichVuId);

        return t;
    }

    private static QuyThuChi CreateChi(
        string maGiaoDich,
        DateTimeOffset ngay,
        PhuongThucThanhToan phuongThuc,
        string nguoiGiaoDich,
        string? chungTu,
        IEnumerable<(string nhomThongKe, decimal soTien, string? ghiChu, int? dichVuId)> chiTiets)
    {
        var result = QuyThuChi.CreateChi(maGiaoDich, ngay, phuongThuc, nguoiGiaoDich, chungTu);
        if (result.IsFailure) throw new Exception($"Seed QuyChi failed [{maGiaoDich}]: {result.Errors.First().Description}");

        var c = result.Value;
        foreach (var (nhomThongKe, soTien, ghiChu, dichVuId) in chiTiets)
            c.AddChiTiet(soTien, nhomThongKe, ghiChu, dichVuId);

        return c;
    }
}
