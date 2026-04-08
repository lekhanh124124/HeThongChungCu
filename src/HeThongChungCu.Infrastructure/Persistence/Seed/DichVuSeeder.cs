using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class DichVuSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.DichVus.AnyAsync()) return;

        logger.LogInformation("Seeding Mandatory Services and Partners...");

        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        // --- 1. Tạo các Đối tác cung cấp nguồn ---
        var evn = new DoiTac(
            "EVN TP.HCM",
            "Tổng Công ty Điện lực Thành phố Hồ Chí Minh",
            "Trần Văn Điện",
            "GPKD-EVN-001",
            "MST-0300465569",
            "356 Lý Tự Trọng, Quận 1, TP. Hồ Chí Minh",
            "1900545454",
            "cskh@evnhcmc.vn",
            "Nhà cung cấp điện năng đầu nguồn tại TP.HCM.");

        var sawaco = new DoiTac(
            "SAWACO",
            "Tổng Công ty Cấp nước Sài Gòn",
            "Lê Văn Nước",
            "GPKD-SW-002",
            "MST-0304169542",
            "01 Công trường Quốc tế, Phường Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh",
            "19001069",
            "cskh@sawaco.com.vn",
            "Nhà cung cấp nước sạch đầu nguồn tại TP.HCM.");

        var citenco = new DoiTac(
            "CITENCO",
            "Công ty TNHH MTV Môi trường Đô thị TP.HCM",
            "Nguyễn Văn A",
            "GPKD-789012",
            "MST-0301445724",
            "42 Hiệp Nhất, Phường 4, Quận Tân Bình, TP. Hồ Chí Minh",
            "02838443831",
            "info@citenco.com.vn",
            "Đối tác thu gom rác thải mặc định tại TP.HCM.");

        var california = new DoiTac(
            "California Fitness",
            "Công ty TNHH California Fitness & Yoga",
            "Randy Dobson",
            "GPKD-CALI-001",
            "MST-0305040152",
            "Lầu 3, 126 Hùng Vương, Quận 5, TP. Hồ Chí Minh",
            "18006995",
            "info@cfyc.com.vn",
            "Đối tác cung cấp dịch vụ Gym & Yoga cao cấp.");

        var cleany = new DoiTac(
            "Cleany",
            "Hệ thống Giặt ủi Cleany",
            "Nguyễn Văn Giặt",
            "GPKD-CLEAN-002",
            "MST-0315442211",
            "200 Nguyễn Thị Minh Khai, Quận 3, TP. Hồ Chí Minh",
            "19001234",
            "contact@cleany.vn",
            "Đối tác cung cấp dịch vụ giặt ủi công nghiệp và dân dụng.");

        var btaskee = new DoiTac(
            "bTaskee",
            "Công ty TNHH bTaskee",
            "Nathan Do",
            "GPKD-BTASK-003",
            "MST-0313718534",
            "284/25/3 Lý Thường Kiệt, Quận 10, TP. Hồ Chí Minh",
            "1900636736",
            "support@btaskee.com",
            "Đối tác cung cấp dịch vụ giúp việc gia đình qua ứng dụng.");

        if (adminId != 0)
        {
            evn.SetCreated(adminId, DateTimeOffset.UtcNow);
            sawaco.SetCreated(adminId, DateTimeOffset.UtcNow);
            citenco.SetCreated(adminId, DateTimeOffset.UtcNow);
            california.SetCreated(adminId, DateTimeOffset.UtcNow);
            cleany.SetCreated(adminId, DateTimeOffset.UtcNow);
            btaskee.SetCreated(adminId, DateTimeOffset.UtcNow);
        }

        await context.DoiTacs.AddRangeAsync(evn, sawaco, citenco, california, cleany, btaskee);
        await context.SaveChangesAsync();

        // --- 2. Khởi tạo các Dịch vụ ---

        // 2.1. Dịch vụ Vận hành (Nội bộ 100%)
        var dvVanHanh = new DichVu(
            "DV_VANHANH",
            "Dịch vụ vận hành tòa nhà",
            LoaiDichVu.QuanLy,
            "m2",
            "Phí quản lý vận hành, bảo trì hạ tầng chung của tòa nhà.",
            null,
            true);

        if (adminId != 0) dvVanHanh.SetCreated(adminId, DateTimeOffset.UtcNow);

        await context.DichVus.AddAsync(dvVanHanh);
        await context.SaveChangesAsync();

        var bgVanHanh = new BangGiaLoaiCanHo(dvVanHanh.Id, "Bảng giá vận hành 2026", DateTimeOffset.UtcNow);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Standard, 10000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Studio, 8000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Penthouse, 25000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Shophouse, 20000);

        if (adminId != 0) bgVanHanh.SetCreated(adminId, DateTimeOffset.UtcNow);

        await context.BangGias.AddAsync(bgVanHanh);

        // 2.2. Dịch vụ Điện lực (Nội bộ quản lý - Có đối tác nguồn)
        var dvDien = new DichVu("DV_DIEN", "Dịch vụ điện lực", LoaiDichVu.Dien, "kWh", "Điện năng sinh hoạt cư dân.", null, true);
        if (adminId != 0) dvDien.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvDien);
        await context.SaveChangesAsync();

        var bgDien = new BangGiaLuyTien(dvDien.Id, "Biểu giá điện sinh hoạt 2026", DateTimeOffset.UtcNow);
        bgDien.AddChiTietGia(0, 50, 1806);
        bgDien.AddChiTietGia(50, 100, 1866);
        bgDien.AddChiTietGia(100, 200, 2167);
        bgDien.AddChiTietGia(200, 300, 2729);
        bgDien.AddChiTietGia(300, 400, 3050);
        bgDien.AddChiTietGia(400, null, 3151);
        if (adminId != 0) bgDien.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgDien);

        // Ký hợp đồng tổng với EVN
        var hdDien = evn.KyHopDongMoi("HD-EVN-2026", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10), 1000000000, dvDien.Id, "Hợp đồng mua điện năng tổng cho tòa nhà.");
        if (adminId != 0) hdDien.SetCreated(adminId, DateTimeOffset.UtcNow);

        // 2.3. Dịch vụ Nước sinh hoạt (Nội bộ quản lý - Có đối tác nguồn)
        var dvNuoc = new DichVu("DV_NUOC", "Dịch vụ nước sinh hoạt", LoaiDichVu.Nuoc, "m3", "Nước sạch sinh hoạt cư dân.", null, true);
        if (adminId != 0) dvNuoc.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvNuoc);
        await context.SaveChangesAsync();

        var bgNuoc = new BangGiaLuyTien(dvNuoc.Id, "Giá nước sinh hoạt 2026", DateTimeOffset.UtcNow);
        bgNuoc.AddChiTietGia(0, 10, 5973);
        bgNuoc.AddChiTietGia(10, 20, 7052);
        bgNuoc.AddChiTietGia(20, 30, 8669);
        bgNuoc.AddChiTietGia(30, null, 15929);
        if (adminId != 0) bgNuoc.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgNuoc);

        // Ký hợp đồng tổng với SAWACO
        var hdNuoc = sawaco.KyHopDongMoi("HD-SW-2026", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5), 500000000, dvNuoc.Id, "Hợp đồng cấp nước sạch toàn khu dân cư.");
        if (adminId != 0) hdNuoc.SetCreated(adminId, DateTimeOffset.UtcNow);

        // 2.4. Dịch vụ Trông giữ phương tiện (Nội bộ)
        var parkingServices = new[]
        {
            (Code: LoaiPhuongTien.XeMay.DefaultServiceCode, Name: "Dịch vụ trông xe máy", Price: 120000m),
            (Code: LoaiPhuongTien.Oto.DefaultServiceCode, Name: "Dịch vụ trông xe ô tô", Price: 1200000m),
            (Code: LoaiPhuongTien.XeDap.DefaultServiceCode, Name: "Dịch vụ trông xe đạp", Price: 30000m)
        };

        foreach (var (Code, Name, Price) in parkingServices)
        {
            var dvParking = new DichVu(Code, Name, LoaiDichVu.PhuongTien, "Xe", Name, null, true);
            if (adminId != 0) dvParking.SetCreated(adminId, DateTimeOffset.UtcNow);
            await context.DichVus.AddAsync(dvParking);
            await context.SaveChangesAsync();
            var bgParking = new BangGiaCoDinh(dvParking.Id, "Giá gửi xe tháng 2026", DateTimeOffset.UtcNow, Price);
            if (adminId != 0) bgParking.SetCreated(adminId, DateTimeOffset.UtcNow);
            await context.BangGias.AddAsync(bgParking);
        }

        // 2.5. Dịch vụ Thu gom rác thải (Thuê ngoài trọn gói)
        var dvRac = new DichVu("DV_RAC", "Dịch vụ thu gom rác thải", LoaiDichVu.Khac, "Hộ", "Phí vệ sinh định kỳ hàng tháng.", null, true);
        if (adminId != 0) dvRac.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvRac);
        await context.SaveChangesAsync();

        var bgRac = new BangGiaCoDinh(dvRac.Id, "Giá phí vệ sinh 2026", DateTimeOffset.UtcNow, 30000);
        if (adminId != 0) bgRac.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgRac);

        // Ký hợp đồng tổng với CITENCO
        var hdRac = citenco.KyHopDongMoi("HD-RAC-2026-001", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1), 50000000, dvRac.Id, "Hợp đồng thu gom rác thải khu vực tòa nhà.");
        if (adminId != 0) hdRac.SetCreated(adminId, DateTimeOffset.UtcNow);

        // --- 3. Dịch vụ Tiện ích & Giá trị gia tăng (Không bắt buộc - Nội bộ) ---

        // 3.1. Gói tập Gym tháng
        var dvGym = new DichVu("DV_GYM", "Dịch vụ Gym", LoaiDichVu.TienIch, "Tháng", "Gói tập Gym đầy đủ trang thiết bị tại tầng tiện ích.", null, false);
        if (adminId != 0) dvGym.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvGym);
        await context.SaveChangesAsync();

        var bgGym = new BangGiaCoDinh(dvGym.Id, "Bảng giá tập Gym 2026", DateTimeOffset.UtcNow, 500000);
        if (adminId != 0) bgGym.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgGym);

        // Ký hợp đồng với California
        var hdGym = california.KyHopDongMoi("HD-CALI-2026", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(3), 200000000, dvGym.Id, "Hợp đồng cung cấp dịch vụ Gym & Yoga cho tòa nhà.");
        if (adminId != 0) hdGym.SetCreated(adminId, DateTimeOffset.UtcNow);

        // 3.2. Vé hồ bơi theo lượt
        var dvPool = new DichVu("DV_POOL", "Vé hồ bơi", LoaiDichVu.TienIch, "Lượt", "Sử dụng hồ bơi vô cực tại tầng thượng.", null, false);
        if (adminId != 0) dvPool.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvPool);
        await context.SaveChangesAsync();

        var bgPool = new BangGiaCoDinh(dvPool.Id, "Bảng giá hồ bơi 2026", DateTimeOffset.UtcNow, 30000);
        if (adminId != 0) bgPool.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgPool);

        // 3.3. Khu vực BBQ (Đặt theo khung giờ)
        var dvBbq = new DichVu("DV_BBQ", "Khu vực BBQ", LoaiDichVu.TienIch, "Slot", "Đặt chỗ tổ chức tiệc BBQ ngoài trời.", null, false);
        if (adminId != 0) dvBbq.SetCreated(adminId, DateTimeOffset.UtcNow);

        dvBbq.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Sáng (08:00 - 12:00)");
        dvBbq.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Chiều (13:00 - 17:00)");
        dvBbq.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Tối (18:00 - 22:00)");

        await context.DichVus.AddAsync(dvBbq);
        await context.SaveChangesAsync(); // Lưu để lấy ID KhungGio

        var bgBbq = new BangGiaKhungGio(dvBbq.Id, "Giá thuê sân BBQ 2026", DateTimeOffset.UtcNow);
        if (adminId != 0) bgBbq.SetCreated(adminId, DateTimeOffset.UtcNow);

        foreach (var kg in dvBbq.KhungGios)
        {
            bgBbq.AddGiaKhungGio(kg.Id, 200000);
        }
        await context.BangGias.AddAsync(bgBbq);

        // 3.4. Phòng sinh hoạt cộng đồng
        var dvCommon = new DichVu("DV_COMMUNITY", "Phòng sinh hoạt cộng đồng", LoaiDichVu.TienIch, "Slot", "Sử dụng phòng sinh hoạt cho các sự kiện cá nhân.", null, false);
        if (adminId != 0) dvCommon.SetCreated(adminId, DateTimeOffset.UtcNow);

        dvCommon.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Ca Sáng");
        dvCommon.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Ca Chiều");
        dvCommon.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Ca Tối");

        await context.DichVus.AddAsync(dvCommon);
        await context.SaveChangesAsync();

        var bgCommon = new BangGiaKhungGio(dvCommon.Id, "Giá thuê phòng cộng đồng 2026", DateTimeOffset.UtcNow);
        if (adminId != 0) bgCommon.SetCreated(adminId, DateTimeOffset.UtcNow);

        foreach (var kg in dvCommon.KhungGios)
        {
            bgCommon.AddGiaKhungGio(kg.Id, 100000);
        }
        await context.BangGias.AddAsync(bgCommon);

        // 3.5. Dịch vụ Giặt ủi
        var dvLaundry = new DichVu("DV_LAUNDRY", "Dịch vụ giặt ủi", LoaiDichVu.Khac, "Kg", "Giặt sấy quần áo tận tâm.", null, false);
        if (adminId != 0) dvLaundry.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvLaundry);
        await context.SaveChangesAsync();

        var bgLaundry = new BangGiaCoDinh(dvLaundry.Id, "Bảng giá giặt ủi 2026", DateTimeOffset.UtcNow, 20000);
        if (adminId != 0) bgLaundry.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgLaundry);

        // Ký hợp đồng với Cleany
        var hdLaundry = cleany.KyHopDongMoi("HD-CLEAN-2026", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1), 50000000, dvLaundry.Id, "Hợp đồng cung cấp dịch vụ giặt ủi định kỳ.");
        if (adminId != 0) hdLaundry.SetCreated(adminId, DateTimeOffset.UtcNow);

        // 3.6. Dịch vụ giúp việc theo giờ
        var dvCleaning = new DichVu("DV_CLEANING", "Dịch vụ dọn dẹp", LoaiDichVu.Khac, "Giờ", "Vệ sinh căn hộ theo yêu cầu.", null, false);
        if (adminId != 0) dvCleaning.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.DichVus.AddAsync(dvCleaning);
        await context.SaveChangesAsync();

        var bgCleaning = new BangGiaCoDinh(dvCleaning.Id, "Bảng giá dọn dẹp 2026", DateTimeOffset.UtcNow, 100000);
        if (adminId != 0) bgCleaning.SetCreated(adminId, DateTimeOffset.UtcNow);
        await context.BangGias.AddAsync(bgCleaning);

        // Ký hợp đồng với bTaskee
        var hdCleaning = btaskee.KyHopDongMoi("HD-BTASK-2026", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1), 30000000, dvCleaning.Id, "Hợp đồng hợp tác cung cấp nhân sự giúp việc qua ứng dụng.");
        if (adminId != 0) hdCleaning.SetCreated(adminId, DateTimeOffset.UtcNow);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Mandatory Services and Partners Seeded Successfully.");
    }
}
