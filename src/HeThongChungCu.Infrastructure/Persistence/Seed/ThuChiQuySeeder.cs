using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Bogus;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class QuyThuChiSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding QuyThuChi financial transaction logs...");

        if (await context.QuyThuChis.AnyAsync())
        {
            logger.LogInformation("QuyThuChi already seeded. Skipping.");
            return;
        }

        var faker = new Faker("vi");

        var dichVus = await context.DichVus.ToDictionaryAsync(d => d.MaDichVu, d => (int?)d.Id, StringComparer.OrdinalIgnoreCase);

        int? GetDv(string code)
        {
            if (dichVus.TryGetValue(code, out var id)) return id;
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
        var dvPool = GetDv("DV_POOL");
        var dvBBQ = GetDv("DV_BBQ");

        var list = new List<QuyThuChi>();
        var gio7 = TimeSpan.FromHours(7);

        int receiptCounter = 1;
        int paymentCounter = 1;

        for (int year = 2025; year <= 2026; year++)
        {
            int endMonth = year == 2026 ? 5 : 12;
            for (int month = 1; month <= endMonth; month++)
            {
                // THU ĐẶC THÙ (Special Income)
                // 1. Tiện ích (BBQ, Hồ bơi, Tennis)
                var tienTienIch = faker.Random.Decimal(3_000_000m, 15_000_000m);
                list.Add(CreateThu($"THU-{year}{month:D2}-{receiptCounter++:D3}",
                    new DateTimeOffset(year, month, faker.Random.Int(5, 15), 10, 0, 0, gio7),
                    PhuongThucThanhToan.ChuyenKhoan,
                    "Cư dân đặt tiện ích",
                    null,
                    chiTiets: new[] {
                        (dvBBQ ?? dvPool ?? dvTennis, tienTienIch, "Thu phí sử dụng tiện ích", $"Phòng SHCD, BBQ, Hồ bơi T{month}/{year}")
                    }));

                // 2. Thu khác (Quảng cáo)
                if (faker.Random.Bool(0.7f)) // 70% có thu quảng cáo
                {
                    var tienQuangCao = faker.Random.Decimal(5_000_000m, 12_000_000m);
                    list.Add(CreateThu($"THU-{year}{month:D2}-{receiptCounter++:D3}",
                        new DateTimeOffset(year, month, faker.Random.Int(16, 25), 14, 0, 0, gio7),
                        PhuongThucThanhToan.ChuyenKhoan,
                        "Công ty Quảng cáo",
                        $"HD-QC-{year}{month}",
                        chiTiets: new[] {
                            ((int?)null, tienQuangCao, "Thu khác", $"Quảng cáo màn hình thang máy T{month}/{year}")
                        }));
                }

                // 3. Đặt cọc thi công (random)
                if (faker.Random.Bool(0.4f)) // 40% có thu cọc
                {
                    list.Add(CreateThu($"THU-{year}{month:D2}-{receiptCounter++:D3}",
                        new DateTimeOffset(year, month, faker.Random.Int(1, 28), 9, 30, 0, gio7),
                        PhuongThucThanhToan.ChuyenKhoan,
                        $"Cư dân {faker.Name.FullName()}",
                        $"UNC-THC-{year}{month}",
                        chiTiets: new[] {
                            ((int?)null, 20_000_000m, "Thu đặt cọc thi công", $"Ký quỹ sửa nội thất căn hộ")
                        }));
                }

                // 4. Thu bù đắp (để cân bằng với chi phí lớn do không seed đủ 100% căn hộ)
                var thuBuDapQuanLy = faker.Random.Decimal(90_000_000m, 120_000_000m);
                var thuBuDapDien = faker.Random.Decimal(30_000_000m, 50_000_000m);
                list.Add(CreateThu($"THU-{year}{month:D2}-{receiptCounter++:D3}",
                    new DateTimeOffset(year, month, faker.Random.Int(18, 28), 16, 0, 0, gio7),
                    PhuongThucThanhToan.TienMat,
                    "Đại diện các chủ hộ",
                    $"PN-T{month}-{year}",
                    chiTiets: new[] {
                        (dvManagement, thuBuDapQuanLy, "Thu phí quản lý vận hành", $"Thu phí quản lý T{month}/{year} (thu ngoài hệ thống)"),
                        (dvElectric, thuBuDapDien, "Thu tiền điện", $"Thu tiền điện T{month}/{year} (thu ngoài hệ thống)")
                    }));

                // CHI VẬN HÀNH (Expenses)
                // 1. Điện tổng tòa nhà
                var tienDienTong = faker.Random.Decimal(80_000_000m, 120_000_000m);
                list.Add(CreateChi($"CHI-{year}{month:D2}-{paymentCounter++:D3}",
                    new DateTimeOffset(year, month, 20, 10, 0, 0, gio7),
                    PhuongThucThanhToan.ChuyenKhoan,
                    "Công ty Điện lực EVN",
                    $"HD-EVN-{year}{month}",
                    chiTiets: new[] {
                        ("Chi vận hành", tienDienTong, $"Điện tổng tòa nhà + sinh hoạt T{month}/{year}", dvElectric)
                    }));

                // 2. Nước tổng
                var tienNuocTong = faker.Random.Decimal(25_000_000m, 40_000_000m);
                list.Add(CreateChi($"CHI-{year}{month:D2}-{paymentCounter++:D3}",
                    new DateTimeOffset(year, month, 22, 10, 0, 0, gio7),
                    PhuongThucThanhToan.ChuyenKhoan,
                    "Công ty Cấp nước Sawaco",
                    $"HD-NUOC-{year}{month}",
                    chiTiets: new[] {
                        ("Chi vận hành", tienNuocTong, $"Tiền nước sinh hoạt + xịt rửa T{month}/{year}", dvWater)
                    }));

                // 3. Lương nhân viên
                var tienLuong = faker.Random.Decimal(90_000_000m, 110_000_000m);
                list.Add(CreateChi($"CHI-{year}{month:D2}-{paymentCounter++:D3}",
                    new DateTimeOffset(year, month, 5, 10, 0, 0, gio7),
                    PhuongThucThanhToan.ChuyenKhoan,
                    "Nhân viên BQL",
                    $"LUONG-{year}{month}",
                    chiTiets: new[] {
                        ("Lương nhân viên", tienLuong, $"Trả lương nhân viên BQL T{month}/{year}", (int?)null)
                    }));

                // 4. Bảo trì thang máy
                list.Add(CreateChi($"CHI-{year}{month:D2}-{paymentCounter++:D3}",
                    new DateTimeOffset(year, month, 25, 9, 0, 0, gio7),
                    PhuongThucThanhToan.ChuyenKhoan,
                    "Schindler Việt Nam",
                    $"HD-SCH-{year}{month}",
                    chiTiets: new[] {
                        ("Chi trả nhà cung cấp/đối tác", 15_000_000m, $"Bảo trì thang máy T{month}/{year}", (int?)null)
                    }));

                // 5. Chi phí linh tinh (VPP, In ấn, Cảnh quan)
                var chiTietLinhTinh = new List<(string, decimal, string?, int?)>();
                chiTietLinhTinh.Add(("Văn phòng phẩm", faker.Random.Decimal(500_000m, 1_500_000m), "Bút, giấy in cho BQL", null));
                chiTietLinhTinh.Add(("Chi trả đối tác viễn thông", faker.Random.Decimal(2_000_000m, 5_000_000m), "Cáp quang Viettel", dvInternet));
                
                if (faker.Random.Bool(0.5f)) // 50% tháng có chi chăm sóc cây cảnh
                {
                    chiTietLinhTinh.Add(("Cảnh quan, vệ sinh", faker.Random.Decimal(3_000_000m, 8_000_000m), "Chăm sóc cây xanh sảnh chính", null));
                }

                list.Add(CreateChi($"CHI-{year}{month:D2}-{paymentCounter++:D3}",
                    new DateTimeOffset(year, month, 28, 14, 0, 0, gio7),
                    PhuongThucThanhToan.TienMat,
                    "Các nhà cung cấp",
                    $"HD-OTHER-{year}{month}",
                    chiTiets: chiTietLinhTinh));
            }
        }

        await context.QuyThuChis.AddRangeAsync(list);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {list.Count} QuyThuChi records covering 01/2025 to 05/2026.");
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
