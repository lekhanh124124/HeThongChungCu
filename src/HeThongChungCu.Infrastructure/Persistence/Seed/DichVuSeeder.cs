
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
            "Trần Thế Vinh",
            "GPKD-EVN-001",
            "MST-0300465569",
            "356 Lý Tự Trọng, Quận 1, TP. Hồ Chí Minh",
            "1900545454",
            "cskh@evnhcmc.vn",
            "Nhà cung cấp điện năng đầu nguồn tại TP.HCM.");

        var sawaco = new DoiTac(
            "SAWACO",
            "Tổng Công ty Cấp nước Sài Gòn",
            "Lê Hoàng Nam",
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
            "Nguyễn Minh Tiến",
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

        var viettel = new DoiTac(
            "Viettel Telecom",
            "Tổng Công ty Viễn thông Viettel",
            "Tào Đức Thắng",
            "GPKD-VT-001",
            "MST-0100109106",
            "Số 1 Giang Văn Minh, Kim Mã, Ba Đình, Hà Nội",
            "18008119",
            "cskh@viettel.com.vn",
            "Đối tác cung cấp hạ tầng Internet và Truyền hình cáp.");

        var washup = new DoiTac(
            "WashUp",
            "Chuỗi rửa xe thông minh WashUp",
            "Lê Văn Nam",
            "GPKD-WU-001",
            "MST-0316442211",
            "12 Phan Kế Bính, Quận 1, TP. Hồ Chí Minh",
            "19001234",
            "contact@washup.vn",
            "Đối tác cung cấp dịch vụ rửa xe thông minh tận nơi.");

        var lavie = new DoiTac(
            "La Vie",
            "Công ty TNHH La Vie",
            "Fausto Tazzi",
            "GPKD-LAVIE-001",
            "MST-0300530438",
            "Quốc lộ 1A, Phường Khánh Hậu, TP. Tân An, Long An",
            "19001906",
            "cskh@laviewater.com",
            "Đối tác cung cấp nước uống tinh khiết.");

        var shiseido = new DoiTac(
            "Shiseido Spa",
            "Công ty TNHH Mỹ phẩm Shiseido Việt Nam",
            "Isao Shirasu",
            "GPKD-SHI-001",
            "MST-0301445724",
            "Lầu 27, Vietcombank Tower, Quận 1, TP. Hồ Chí Minh",
            "02839101221",
            "info@shiseido.com.vn",
            "Đối tác cung cấp dịch vụ Spa & Massage cao cấp.");

        var schindler = new DoiTac(
            "Schindler Vietnam",
            "Công ty TNHH Schindler Việt Nam",
            "Dương Thành Nam",
            "GPKD-SCH-001",
            "MST-0301445724",
            "Lầu 8, President Place, 93 Nguyễn Du, Quận 1, TP. Hồ Chí Minh",
            "02835214000",
            "info@vn.schindler.com",
            "Đối tác bảo trì thang máy chuyên nghiệp.");

        var daikin = new DoiTac(
            "Daikin Vietnam",
            "Công ty Cổ phần Daikin Air Conditioning (Vietnam)",
            "Lý Thị Phương Hoa",
            "GPKD-DK-001",
            "MST-0304323145",
            "Lầu 12, Tòa nhà Nam Á, 201-203 Cách Mạng Tháng Tám, Quận 3, TP. Hồ Chí Minh",
            "18006777",
            "info@daikin.com.vn",
            "Đối tác cung cấp và bảo trì hệ thống điều hòa trung tâm VRV.");

        if (adminId != 0)
        {
            evn.SetCreated(adminId, DateTimeOffset.Now);
            sawaco.SetCreated(adminId, DateTimeOffset.Now);
            citenco.SetCreated(adminId, DateTimeOffset.Now);
            california.SetCreated(adminId, DateTimeOffset.Now);
            cleany.SetCreated(adminId, DateTimeOffset.Now);
            btaskee.SetCreated(adminId, DateTimeOffset.Now);
            viettel.SetCreated(adminId, DateTimeOffset.Now);
            washup.SetCreated(adminId, DateTimeOffset.Now);
            lavie.SetCreated(adminId, DateTimeOffset.Now);
            shiseido.SetCreated(adminId, DateTimeOffset.Now);
            schindler.SetCreated(adminId, DateTimeOffset.Now);
            daikin.SetCreated(adminId, DateTimeOffset.Now);
        }

        await context.DoiTacs.AddRangeAsync(evn, sawaco, citenco, california, cleany, btaskee, viettel, washup, lavie, shiseido, schindler, daikin);
        await context.SaveChangesAsync();

        // --- 2. Khởi tạo các Dịch vụ ---

        // 2.1. Dịch vụ Vận hành (Nội bộ 100%)
        var dvVanHanh = new DichVu(
            "MANAGEMENT_FEE",
            "Dịch vụ vận hành tòa nhà",
            LoaiDichVu.VanHanh,
            "m2",
            "Phí quản lý vận hành, bảo trì hạ tầng chung của tòa nhà.",
            null,
            true);
        dvVanHanh.Activate();

        if (adminId != 0) dvVanHanh.SetCreated(adminId, DateTimeOffset.Now);

        await context.DichVus.AddAsync(dvVanHanh);
        await context.SaveChangesAsync();

        var bgVanHanh = new BangGiaLoaiCanHo(dvVanHanh.Id, "Bảng giá vận hành 2026", DateTimeOffset.Now, true);
        bgVanHanh.Activate();
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Standard, 10000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Studio, 8000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Penthouse, 25000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Shophouse, 20000);

        if (adminId != 0) bgVanHanh.SetCreated(adminId, DateTimeOffset.Now);

        await context.BangGias.AddAsync(bgVanHanh);
        await context.SaveChangesAsync();

        // 2.2. Dịch vụ Điện lực (Nội bộ quản lý - Có đối tác nguồn)
        var dvDien = new DichVu("ELECTRICITY", "Dịch vụ điện lực", LoaiDichVu.VanHanh, "kWh", "Điện năng sinh hoạt cư dân.", null, true);
        dvDien.Activate();
        if (adminId != 0) dvDien.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvDien);
        await context.SaveChangesAsync();

        var bgDien = new BangGiaLuyTien(dvDien.Id, "Biểu giá điện sinh hoạt 2026", DateTimeOffset.Now, true);
        bgDien.Activate();
        bgDien.AddChiTietGia(0, 50, 1806);
        bgDien.AddChiTietGia(50, 100, 1866);
        bgDien.AddChiTietGia(100, 200, 2167);
        bgDien.AddChiTietGia(200, 300, 2729);
        bgDien.AddChiTietGia(300, 400, 3050);
        bgDien.AddChiTietGia(400, null, 3151);
        if (adminId != 0) bgDien.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgDien);
        await context.SaveChangesAsync();

        // Ký hợp đồng tổng với EVN
        var hdDien = evn.KyHopDongMoi("HD-EVN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10), 20000000000, dvDien.Id, "Hợp đồng mua điện năng tổng cho tòa nhà.");
        if (adminId != 0) hdDien.SetCreated(adminId, DateTimeOffset.Now);

        // 2.3. Dịch vụ Nước sinh hoạt (Nội bộ quản lý - Có đối tác nguồn)
        var dvNuoc = new DichVu("WATER", "Dịch vụ nước sinh hoạt", LoaiDichVu.VanHanh, "m3", "Nước sạch sinh hoạt cư dân.", null, true);
        dvNuoc.Activate();
        if (adminId != 0) dvNuoc.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvNuoc);
        await context.SaveChangesAsync();

        var bgNuoc = new BangGiaLuyTien(dvNuoc.Id, "Giá nước sinh hoạt 2026", DateTimeOffset.Now, true);
        bgNuoc.Activate();
        bgNuoc.AddChiTietGia(0, 10, 5973);
        bgNuoc.AddChiTietGia(10, 20, 7052);
        bgNuoc.AddChiTietGia(20, 30, 8669);
        bgNuoc.AddChiTietGia(30, null, 15929);
        if (adminId != 0) bgNuoc.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgNuoc);
        await context.SaveChangesAsync();

        // Ký hợp đồng tổng với SAWACO
        var hdNuoc = sawaco.KyHopDongMoi("HD-SW-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), 3000000000, dvNuoc.Id, "Hợp đồng cấp nước sạch toàn khu dân cư.");
        if (adminId != 0) hdNuoc.SetCreated(adminId, DateTimeOffset.Now);

        // 2.4. Dịch vụ Internet & Truyền hình (Vận hành - Bắt buộc)
        var dvInternet = new DichVu("INTERNET_BASIC", "Gói Internet & Truyền hình cơ bản", LoaiDichVu.VanHanh, "Tháng", "Hạ tầng Internet tốc độ cao và Truyền hình cáp cho toàn căn hộ.", null, true);
        dvInternet.Activate();
        if (adminId != 0) dvInternet.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvInternet);
        await context.SaveChangesAsync();

        var bgInternet = new BangGiaCoDinh(dvInternet.Id, "Bảng giá Internet 2026", DateTimeOffset.Now, 165000, true);
        bgInternet.Activate();
        if (adminId != 0) bgInternet.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgInternet);
        await context.SaveChangesAsync();

        // Ký hợp đồng với Viettel
        var hdInternet = viettel.KyHopDongMoi("HD-VT-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), 500000000, dvInternet.Id, "Hợp đồng cung cấp hạ tầng viễn thông cho tòa nhà.");
        if (adminId != 0) hdInternet.SetCreated(adminId, DateTimeOffset.Now);

        // 2.5. Dịch vụ Trông giữ phương tiện (Nội bộ)
        var parkingServices = new[]
        {
            (Code: LoaiPhuongTien.XeMay.DefaultServiceCode, Name: "Dịch vụ giữ xe máy", Price: 120000m),
            (Code: LoaiPhuongTien.Oto.DefaultServiceCode, Name: "Dịch vụ giữ xe ô tô", Price: 1200000m),
            (Code: LoaiPhuongTien.XeDap.DefaultServiceCode, Name: "Dịch vụ giữ xe đạp", Price: 30000m)
        };

        foreach (var (Code, Name, Price) in parkingServices)
        {
            var dvParking = new DichVu(Code, Name, LoaiDichVu.VanHanh, "Xe", Name, null, false);
            dvParking.Activate();
            if (adminId != 0) dvParking.SetCreated(adminId, DateTimeOffset.Now);
            await context.DichVus.AddAsync(dvParking);
            await context.SaveChangesAsync();
            var bgParking = new BangGiaCoDinh(dvParking.Id, "Giá giữ xe tháng 2026", DateTimeOffset.Now, Price, true);
            bgParking.Activate();
            if (adminId != 0) bgParking.SetCreated(adminId, DateTimeOffset.Now);
            await context.BangGias.AddAsync(bgParking);
            await context.SaveChangesAsync();
        }

        // 2.6. Dịch vụ Thu gom rác thải (Thuê ngoài trọn gói)
        var dvRac = new DichVu("DV_RAC", "Dịch vụ thu gom rác thải", LoaiDichVu.VanHanh, "Hộ", "Phí vệ sinh định kỳ hàng tháng.", null, true);
        dvRac.Activate();
        if (adminId != 0) dvRac.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvRac);
        await context.SaveChangesAsync();

        var bgRac = new BangGiaCoDinh(dvRac.Id, "Giá phí vệ sinh 2026", DateTimeOffset.Now, 30000, true);
        bgRac.Activate();
        if (adminId != 0) bgRac.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgRac);
        await context.SaveChangesAsync();

        // Ký hợp đồng tổng với CITENCO
        var hdRac = citenco.KyHopDongMoi("HD-RAC-2026-001", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 50000000, dvRac.Id, "Hợp đồng thu gom rác thải khu vực tòa nhà.");
        if (adminId != 0) hdRac.SetCreated(adminId, DateTimeOffset.Now);

        // --- 3. Dịch vụ Tiện ích & Giá trị gia tăng (Không bắt buộc - Nội bộ) ---

        // 3.1. Gói tập Gym tháng
        var dvGym = new DichVu("DV_GYM", "Dịch vụ Gym", LoaiDichVu.TienIch, "Tháng", "Gói tập Gym đầy đủ trang thiết bị tại tầng tiện ích.", null, false);
        dvGym.Activate();
        if (adminId != 0) dvGym.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvGym);
        await context.SaveChangesAsync();

        var bgGym = new BangGiaCoDinh(dvGym.Id, "Bảng giá tập Gym 2026", DateTimeOffset.Now, 500000, true);
        bgGym.Activate();
        if (adminId != 0) bgGym.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgGym);
        await context.SaveChangesAsync();

        // Ký hợp đồng với California
        var hdGym = california.KyHopDongMoi("HD-CALI-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3), 200000000, dvGym.Id, "Hợp đồng cung cấp dịch vụ Gym & Yoga cho tòa nhà.");
        if (adminId != 0) hdGym.SetCreated(adminId, DateTimeOffset.Now);

        // 3.2. Vé hồ bơi theo lượt
        var dvPool = new DichVu("DV_POOL", "Vé hồ bơi", LoaiDichVu.TienIch, "Lượt", "Sử dụng hồ bơi vô cực tại tầng thượng.", null, false);
        dvPool.Activate();
        if (adminId != 0) dvPool.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvPool);
        await context.SaveChangesAsync();

        var bgPool = new BangGiaCoDinh(dvPool.Id, "Bảng giá hồ bơi 2026", DateTimeOffset.Now, 30000, false);
        bgPool.Activate();
        if (adminId != 0) bgPool.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgPool);
        await context.SaveChangesAsync();

        // 3.3. Khu vực BBQ (Đặt theo khung giờ)
        var dvBbq = new DichVu("DV_BBQ", "Khu vực BBQ", LoaiDichVu.TienIch, "Slot", "Đặt chỗ tổ chức tiệc BBQ ngoài trời.", null, false);
        dvBbq.Activate();
        if (adminId != 0) dvBbq.SetCreated(adminId, DateTimeOffset.Now);

        dvBbq.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Sáng (08:00 - 12:00)").Value.Activate();
        dvBbq.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Chiều (13:00 - 17:00)").Value.Activate();
        dvBbq.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Tối (18:00 - 22:00)").Value.Activate();

        await context.DichVus.AddAsync(dvBbq);
        await context.SaveChangesAsync(); // Lưu để lấy ID KhungGio

        var bgBbq = new BangGiaKhungGio(dvBbq.Id, "Giá thuê sân BBQ 2026", DateTimeOffset.Now, false);
        bgBbq.Activate();
        if (adminId != 0) bgBbq.SetCreated(adminId, DateTimeOffset.Now);

        foreach (var kg in dvBbq.KhungGios)
        {
            bgBbq.AddGiaKhungGio(kg.Id, 200000);
        }
        await context.BangGias.AddAsync(bgBbq);
        await context.SaveChangesAsync();

        // 3.4. Phòng sinh hoạt cộng đồng
        var dvCommon = new DichVu("DV_COMMUNITY", "Phòng sinh hoạt cộng đồng", LoaiDichVu.TienIch, "Slot", "Sử dụng phòng sinh hoạt cho các sự kiện cá nhân.", null, false);
        dvCommon.Activate();
        if (adminId != 0) dvCommon.SetCreated(adminId, DateTimeOffset.Now);

        dvCommon.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Ca Sáng").Value.Activate();
        dvCommon.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Ca Chiều").Value.Activate();
        dvCommon.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Ca Tối").Value.Activate();

        await context.DichVus.AddAsync(dvCommon);
        await context.SaveChangesAsync();

        var bgCommon = new BangGiaKhungGio(dvCommon.Id, "Giá thuê phòng cộng đồng 2026", DateTimeOffset.Now, false);
        bgCommon.Activate();
        if (adminId != 0) bgCommon.SetCreated(adminId, DateTimeOffset.Now);

        foreach (var kg in dvCommon.KhungGios)
        {
            bgCommon.AddGiaKhungGio(kg.Id, 100000);
        }
        await context.BangGias.AddAsync(bgCommon);
        await context.SaveChangesAsync();

        // 3.5. Dịch vụ Giặt ủi
        var dvLaundry = new DichVu("DV_LAUNDRY", "Dịch vụ giặt ủi", LoaiDichVu.TienIch, "Kg", "Giặt sấy quần áo tận tâm.", null, false);
        dvLaundry.Activate();
        if (adminId != 0) dvLaundry.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvLaundry);
        await context.SaveChangesAsync();

        var bgLaundry = new BangGiaCoDinh(dvLaundry.Id, "Bảng giá giặt ủi 2026", DateTimeOffset.Now, 20000, false);
        bgLaundry.Activate();
        if (adminId != 0) bgLaundry.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgLaundry);
        await context.SaveChangesAsync();

        // Ký hợp đồng với Cleany
        var hdLaundry = cleany.KyHopDongMoi("HD-CLEAN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 50000000, dvLaundry.Id, "Hợp đồng cung cấp dịch vụ giặt ủi định kỳ.");
        if (adminId != 0) hdLaundry.SetCreated(adminId, DateTimeOffset.Now);

        // 3.6. Dịch vụ giúp việc theo giờ
        var dvCleaning = new DichVu("DV_CLEANING", "Dịch vụ dọn dẹp", LoaiDichVu.TienIch, "Giờ", "Vệ sinh căn hộ theo yêu cầu.", null, false);
        dvCleaning.Activate();
        if (adminId != 0) dvCleaning.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvCleaning);
        await context.SaveChangesAsync();

        var bgCleaning = new BangGiaCoDinh(dvCleaning.Id, "Bảng giá dọn dẹp 2026", DateTimeOffset.Now, 100000, false);
        bgCleaning.Activate();
        if (adminId != 0) bgCleaning.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgCleaning);
        await context.SaveChangesAsync();

        // Ký hợp đồng với bTaskee
        var hdCleaning = btaskee.KyHopDongMoi("HD-BTASK-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 30000000, dvCleaning.Id, "Hợp đồng hợp tác cung cấp nhân sự giúp việc qua ứng dụng.");
        if (adminId != 0) hdCleaning.SetCreated(adminId, DateTimeOffset.Now);

        // 3.7. Dịch vụ Giao nước uống
        var dvWaterBottle = new DichVu("DV_WATER_BOTTLE", "Nước uống La Vie 20L", LoaiDichVu.TienIch, "Bình", "Cung cấp bình nước tinh khiết tận cửa căn hộ.", null, false);
        dvWaterBottle.Activate();
        if (adminId != 0) dvWaterBottle.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvWaterBottle);
        await context.SaveChangesAsync();

        var bgWaterBottle = new BangGiaCoDinh(dvWaterBottle.Id, "Bảng giá nước La Vie 2026", DateTimeOffset.Now, 65000, false);
        bgWaterBottle.Activate();
        if (adminId != 0) bgWaterBottle.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgWaterBottle);
        await context.SaveChangesAsync();

        // Ký hợp đồng với La Vie
        var hdWater = lavie.KyHopDongMoi("HD-LAVIE-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2), 100000000, dvWaterBottle.Id, "Hợp đồng cung cấp nước uống đóng bình.");
        if (adminId != 0) hdWater.SetCreated(adminId, DateTimeOffset.Now);

        // 3.8. Dịch vụ Rửa xe
        var washServices = new[]
        {
            (Code: "DV_WASH_BIKE", Name: "Rửa xe máy", Price: 25000m),
            (Code: "DV_WASH_CAR", Name: "Rửa xe ô tô", Price: 80000m)
        };

        foreach (var (Code, Name, Price) in washServices)
        {
            var dvWash = new DichVu(Code, Name, LoaiDichVu.TienIch, "Lượt", Name, null, false);
            dvWash.Activate();
            if (adminId != 0) dvWash.SetCreated(adminId, DateTimeOffset.Now);
            await context.DichVus.AddAsync(dvWash);
            await context.SaveChangesAsync();

            var bgWash = new BangGiaCoDinh(dvWash.Id, "Bảng giá rửa xe 2026", DateTimeOffset.Now, Price, false);
            bgWash.Activate();
            if (adminId != 0) bgWash.SetCreated(adminId, DateTimeOffset.Now);
            await context.BangGias.AddAsync(bgWash);
            await context.SaveChangesAsync();

            // Ký hợp đồng với WashUp cho từng dịch vụ
            var hdWash = washup.KyHopDongMoi($"HD-WU-{Code}-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 20000000, dvWash.Id, $"Hợp đồng cung cấp dịch vụ {Name}.");
            if (adminId != 0) hdWash.SetCreated(adminId, DateTimeOffset.Now);
        }

        // 3.9. Sân Tennis
        var dvTennis = new DichVu("DV_TENNIS", "Sân Tennis", LoaiDichVu.TienIch, "Slot", "Thuê sân tennis tiêu chuẩn thi đấu.", null, false);
        dvTennis.Activate();
        if (adminId != 0) dvTennis.SetCreated(adminId, DateTimeOffset.Now);

        dvTennis.AddKhungGio(new TimeSpan(6, 0, 0), new TimeSpan(8, 0, 0), "Sáng sớm (06:00 - 08:00)").Value.Activate();
        dvTennis.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), "Sáng (08:00 - 10:00)").Value.Activate();
        dvTennis.AddKhungGio(new TimeSpan(16, 0, 0), new TimeSpan(18, 0, 0), "Chiều (16:00 - 18:00)").Value.Activate();
        dvTennis.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(20, 0, 0), "Tối (18:00 - 20:00)").Value.Activate();
        dvTennis.AddKhungGio(new TimeSpan(20, 0, 0), new TimeSpan(22, 0, 0), "Khuya (20:00 - 22:00)").Value.Activate();

        await context.DichVus.AddAsync(dvTennis);
        await context.SaveChangesAsync();

        var bgTennis = new BangGiaKhungGio(dvTennis.Id, "Giá thuê sân Tennis 2026", DateTimeOffset.Now, false);
        bgTennis.Activate();
        if (adminId != 0) bgTennis.SetCreated(adminId, DateTimeOffset.Now);

        foreach (var kg in dvTennis.KhungGios)
        {
            var price = kg.TenKhungGio.Contains("Tối") || kg.TenKhungGio.Contains("Khuya") ? 250000 : 150000;
            bgTennis.AddGiaKhungGio(kg.Id, price);
        }
        await context.BangGias.AddAsync(bgTennis);
        await context.SaveChangesAsync();

        // 3.10. Dịch vụ Spa & Massage
        var dvSpa = new DichVu("DV_SPA", "Dịch vụ Spa & Massage", LoaiDichVu.TienIch, "Liệu trình", "Chăm sóc sức khỏe và thư giãn.", null, false);
        dvSpa.Activate();
        if (adminId != 0) dvSpa.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvSpa);
        await context.SaveChangesAsync();

        var bgSpa = new BangGiaCoDinh(dvSpa.Id, "Bảng giá Spa 2026", DateTimeOffset.Now, 500000, false);
        bgSpa.Activate();
        if (adminId != 0) bgSpa.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgSpa);
        await context.SaveChangesAsync();

        // Ký hợp đồng với Shiseido
        var hdSpa = shiseido.KyHopDongMoi("HD-SHI-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3), 1000000000, dvSpa.Id, "Hợp đồng cung cấp dịch vụ Spa cao cấp.");
        if (adminId != 0) hdSpa.SetCreated(adminId, DateTimeOffset.Now);

        // --- 4. Dịch vụ Yêu cầu cư dân ---

        // 4.1. Yêu cầu sửa chữa
        var dvYeuCauSuaChua = new DichVu(
            "DV_YC_SUACHUA", 
            "Yêu cầu sửa chữa", 
            LoaiDichVu.YeuCauSuaChua, 
            "Lần", 
            "Dịch vụ xử lý các yêu cầu sửa chữa sự cố trong căn hộ của cư dân.", 
            null, 
            false);
        dvYeuCauSuaChua.Activate();
        if (adminId != 0) dvYeuCauSuaChua.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvYeuCauSuaChua);
        await context.SaveChangesAsync();

        // 4.2. Yêu cầu thi công
        var dvYeuCauThiCong = new DichVu(
            "DV_YC_THICONG", 
            "Yêu cầu thi công", 
            LoaiDichVu.YeuCauThiCong, 
            "Lần", 
            "Dịch vụ xử lý các yêu cầu đăng ký thi công, cải tạo nội thất căn hộ.", 
            null, 
            false);
        dvYeuCauThiCong.Activate();
        if (adminId != 0) dvYeuCauThiCong.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvYeuCauThiCong);
        await context.SaveChangesAsync();

        // --- 5. Dịch vụ Tiền thuê nhà (Dùng BangGiaLoaiCanHo) ---
        var dvThueNha = new DichVu("RENT_FEE", "Tiền thuê nhà", LoaiDichVu.ThueNha, "Tháng", "Tiền thuê căn hộ hàng tháng đối với người thuê.", null, false);
        dvThueNha.Activate();
        if (adminId != 0) dvThueNha.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvThueNha);
        await context.SaveChangesAsync();

        var bgThueNha = new BangGiaLoaiCanHo(dvThueNha.Id, "Bảng giá thuê nhà 2026", DateTimeOffset.Now, true);
        bgThueNha.Activate();
        bgThueNha.AddGiaLoaiCanHo(LoaiCanHo.Standard, 12000000);
        bgThueNha.AddGiaLoaiCanHo(LoaiCanHo.Studio, 7000000);
        bgThueNha.AddGiaLoaiCanHo(LoaiCanHo.Penthouse, 45000000);
        bgThueNha.AddGiaLoaiCanHo(LoaiCanHo.Shophouse, 35000000);
        if (adminId != 0) bgThueNha.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgThueNha);
        await context.SaveChangesAsync();

        // --- 6. Dịch vụ hệ thống: Lãi trễ hạn (Internal — không hiển thị cho cư dân) ---
        // Lãi suất: 0.05%/ngày = 0.0005 (nhân với SoLuong = TongTienGoc × SoNgayQuaHan)
        var dvLaiTreHan = new DichVu(
            "LATE_INTEREST_FEE",
            "Lãi chậm nộp",
            LoaiDichVu.PhatTreHan,
            "VNĐ",
            "Phí lãi phát sinh khi cư dân thanh toán trễ hạn. Tỷ lệ 0.05%/ngày tính trên số tiền gốc.",
            null,
            true);
        dvLaiTreHan.Activate();
        if (adminId != 0) dvLaiTreHan.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvLaiTreHan);
        await context.SaveChangesAsync();

        // BangGiaCoDinh: DonGia = 0.0005 (= 0.05%/ngày)
        // CalculateAmount(context) = DonGia × SoLuong = 0.0005 × (TongTienGoc × SoNgayQuaHan)
        var bgLaiTreHan = new BangGiaCoDinh(dvLaiTreHan.Id, "Lãi suất chậm nộp 2026", DateTimeOffset.Now, 0.0005m, true);
        bgLaiTreHan.Activate();
        if (adminId != 0) bgLaiTreHan.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgLaiTreHan);
        await context.SaveChangesAsync();

        // --- 7. Dịch vụ Bảo trì hạ tầng (Dùng cho đối tác kỹ thuật) ---
        var dvBaoTri = new DichVu("INFRA_MAINTENANCE", "Dịch vụ bảo trì hạ tầng", LoaiDichVu.VanHanh, "Gói", "Dịch vụ bảo trì hệ thống kỹ thuật tòa nhà.", null, true);
        dvBaoTri.Activate();
        if (adminId != 0) dvBaoTri.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvBaoTri);
        await context.SaveChangesAsync();

        // Ký hợp đồng với Schindler và Daikin
        var hdSchindler = schindler.KyHopDongMoi("HD-SCH-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), 1500000000, dvBaoTri.Id, "Hợp đồng bảo trì hệ thống thang máy Otis và Schindler.");
        var hdDaikin = daikin.KyHopDongMoi("HD-DAIKIN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), 800000000, dvBaoTri.Id, "Hợp đồng bảo trì hệ thống điều hòa VRV.");
        
        if (adminId != 0)
        {
            hdSchindler.SetCreated(adminId, DateTimeOffset.Now);
            hdDaikin.SetCreated(adminId, DateTimeOffset.Now);
        }
        await context.HopDongDoiTacs.AddRangeAsync(hdSchindler, hdDaikin);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Mandatory Services and Partners Seeded Successfully.");
    }
}
