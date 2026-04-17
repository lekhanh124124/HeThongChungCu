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

        if (adminId != 0)
        {
            evn.SetCreated(adminId, DateTimeOffset.Now);
            sawaco.SetCreated(adminId, DateTimeOffset.Now);
            citenco.SetCreated(adminId, DateTimeOffset.Now);
            california.SetCreated(adminId, DateTimeOffset.Now);
            cleany.SetCreated(adminId, DateTimeOffset.Now);
            btaskee.SetCreated(adminId, DateTimeOffset.Now);
        }

        await context.DoiTacs.AddRangeAsync(evn, sawaco, citenco, california, cleany, btaskee);
        await context.SaveChangesAsync();

        // --- 2. Khởi tạo các Dịch vụ ---

        // 2.1. Dịch vụ Vận hành (Nội bộ 100%)
        var dvVanHanh = new DichVu(
            "DV_VANHANH",
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

        var bgVanHanh = new BangGiaLoaiCanHo(dvVanHanh.Id, "Bảng giá vận hành 2026", DateTimeOffset.Now);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Standard, 10000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Studio, 8000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Penthouse, 25000);
        bgVanHanh.AddGiaLoaiCanHo(LoaiCanHo.Shophouse, 20000);

        if (adminId != 0) bgVanHanh.SetCreated(adminId, DateTimeOffset.Now);

        await context.BangGias.AddAsync(bgVanHanh);

        // 2.2. Dịch vụ Điện lực (Nội bộ quản lý - Có đối tác nguồn)
        var dvDien = new DichVu("DV_DIEN", "Dịch vụ điện lực", LoaiDichVu.VanHanh, "kWh", "Điện năng sinh hoạt cư dân.", null, true);
        dvDien.Activate();
        if (adminId != 0) dvDien.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvDien);
        await context.SaveChangesAsync();

        var bgDien = new BangGiaLuyTien(dvDien.Id, "Biểu giá điện sinh hoạt 2026", DateTimeOffset.Now);
        bgDien.AddChiTietGia(0, 50, 1806);
        bgDien.AddChiTietGia(50, 100, 1866);
        bgDien.AddChiTietGia(100, 200, 2167);
        bgDien.AddChiTietGia(200, 300, 2729);
        bgDien.AddChiTietGia(300, 400, 3050);
        bgDien.AddChiTietGia(400, null, 3151);
        if (adminId != 0) bgDien.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgDien);

        // Ký hợp đồng tổng với EVN
        var hdDien = evn.KyHopDongMoi("HD-EVN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10), 20000000000, dvDien.Id, "Hợp đồng mua điện năng tổng cho tòa nhà.");
        if (adminId != 0) hdDien.SetCreated(adminId, DateTimeOffset.Now);

        // 2.3. Dịch vụ Nước sinh hoạt (Nội bộ quản lý - Có đối tác nguồn)
        var dvNuoc = new DichVu("DV_NUOC", "Dịch vụ nước sinh hoạt", LoaiDichVu.VanHanh, "m3", "Nước sạch sinh hoạt cư dân.", null, true);
        dvNuoc.Activate();
        if (adminId != 0) dvNuoc.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvNuoc);
        await context.SaveChangesAsync();

        var bgNuoc = new BangGiaLuyTien(dvNuoc.Id, "Giá nước sinh hoạt 2026", DateTimeOffset.Now);
        bgNuoc.AddChiTietGia(0, 10, 5973);
        bgNuoc.AddChiTietGia(10, 20, 7052);
        bgNuoc.AddChiTietGia(20, 30, 8669);
        bgNuoc.AddChiTietGia(30, null, 15929);
        if (adminId != 0) bgNuoc.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgNuoc);

        // Ký hợp đồng tổng với SAWACO
        var hdNuoc = sawaco.KyHopDongMoi("HD-SW-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), 3000000000, dvNuoc.Id, "Hợp đồng cấp nước sạch toàn khu dân cư.");
        if (adminId != 0) hdNuoc.SetCreated(adminId, DateTimeOffset.Now);

        // 2.4. Dịch vụ Trông giữ phương tiện (Nội bộ)
        var parkingServices = new[]
        {
            (Code: LoaiPhuongTien.XeMay.DefaultServiceCode, Name: "Dịch vụ giữ xe máy", Price: 120000m),
            (Code: LoaiPhuongTien.Oto.DefaultServiceCode, Name: "Dịch vụ giữ xe ô tô", Price: 1200000m),
            (Code: LoaiPhuongTien.XeDap.DefaultServiceCode, Name: "Dịch vụ giữ xe đạp", Price: 30000m)
        };

        foreach (var (Code, Name, Price) in parkingServices)
        {
            var dvParking = new DichVu(Code, Name, LoaiDichVu.VanHanh, "Xe", Name, null, true);
            dvParking.Activate();
            if (adminId != 0) dvParking.SetCreated(adminId, DateTimeOffset.Now);
            await context.DichVus.AddAsync(dvParking);
            await context.SaveChangesAsync();
            var bgParking = new BangGiaCoDinh(dvParking.Id, "Giá giữ xe tháng 2026", DateTimeOffset.Now, Price);
            if (adminId != 0) bgParking.SetCreated(adminId, DateTimeOffset.Now);
            await context.BangGias.AddAsync(bgParking);
        }

        // 2.5. Dịch vụ Thu gom rác thải (Thuê ngoài trọn gói)
        var dvRac = new DichVu("DV_RAC", "Dịch vụ thu gom rác thải", LoaiDichVu.VanHanh, "Hộ", "Phí vệ sinh định kỳ hàng tháng.", null, true);
        dvRac.Activate();
        if (adminId != 0) dvRac.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvRac);
        await context.SaveChangesAsync();

        var bgRac = new BangGiaCoDinh(dvRac.Id, "Giá phí vệ sinh 2026", DateTimeOffset.Now, 30000);
        if (adminId != 0) bgRac.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgRac);

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

        var bgGym = new BangGiaCoDinh(dvGym.Id, "Bảng giá tập Gym 2026", DateTimeOffset.Now, 500000);
        if (adminId != 0) bgGym.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgGym);

        // Ký hợp đồng với California
        var hdGym = california.KyHopDongMoi("HD-CALI-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3), 200000000, dvGym.Id, "Hợp đồng cung cấp dịch vụ Gym & Yoga cho tòa nhà.");
        if (adminId != 0) hdGym.SetCreated(adminId, DateTimeOffset.Now);

        // 3.2. Vé hồ bơi theo lượt
        var dvPool = new DichVu("DV_POOL", "Vé hồ bơi", LoaiDichVu.TienIch, "Lượt", "Sử dụng hồ bơi vô cực tại tầng thượng.", null, false);
        dvPool.Activate();
        if (adminId != 0) dvPool.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvPool);
        await context.SaveChangesAsync();

        var bgPool = new BangGiaCoDinh(dvPool.Id, "Bảng giá hồ bơi 2026", DateTimeOffset.Now, 30000);
        if (adminId != 0) bgPool.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgPool);

        // 3.3. Khu vực BBQ (Đặt theo khung giờ)
        var dvBbq = new DichVu("DV_BBQ", "Khu vực BBQ", LoaiDichVu.TienIch, "Slot", "Đặt chỗ tổ chức tiệc BBQ ngoài trời.", null, false);
        dvBbq.Activate();
        if (adminId != 0) dvBbq.SetCreated(adminId, DateTimeOffset.Now);

        dvBbq.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Sáng (08:00 - 12:00)");
        dvBbq.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Chiều (13:00 - 17:00)");
        dvBbq.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Tối (18:00 - 22:00)");

        await context.DichVus.AddAsync(dvBbq);
        await context.SaveChangesAsync(); // Lưu để lấy ID KhungGio

        var bgBbq = new BangGiaKhungGio(dvBbq.Id, "Giá thuê sân BBQ 2026", DateTimeOffset.Now);
        if (adminId != 0) bgBbq.SetCreated(adminId, DateTimeOffset.Now);

        foreach (var kg in dvBbq.KhungGios)
        {
            bgBbq.AddGiaKhungGio(kg.Id, 200000);
        }
        await context.BangGias.AddAsync(bgBbq);

        // 3.4. Phòng sinh hoạt cộng đồng
        var dvCommon = new DichVu("DV_COMMUNITY", "Phòng sinh hoạt cộng đồng", LoaiDichVu.TienIch, "Slot", "Sử dụng phòng sinh hoạt cho các sự kiện cá nhân.", null, false);
        dvCommon.Activate();
        if (adminId != 0) dvCommon.SetCreated(adminId, DateTimeOffset.Now);

        dvCommon.AddKhungGio(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), "Ca Sáng");
        dvCommon.AddKhungGio(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), "Ca Chiều");
        dvCommon.AddKhungGio(new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0), "Ca Tối");

        await context.DichVus.AddAsync(dvCommon);
        await context.SaveChangesAsync();

        var bgCommon = new BangGiaKhungGio(dvCommon.Id, "Giá thuê phòng cộng đồng 2026", DateTimeOffset.Now);
        if (adminId != 0) bgCommon.SetCreated(adminId, DateTimeOffset.Now);

        foreach (var kg in dvCommon.KhungGios)
        {
            bgCommon.AddGiaKhungGio(kg.Id, 100000);
        }
        await context.BangGias.AddAsync(bgCommon);

        // 3.5. Dịch vụ Giặt ủi
        var dvLaundry = new DichVu("DV_LAUNDRY", "Dịch vụ giặt ủi", LoaiDichVu.TienIch, "Kg", "Giặt sấy quần áo tận tâm.", null, false);
        dvLaundry.Activate();
        if (adminId != 0) dvLaundry.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvLaundry);
        await context.SaveChangesAsync();

        var bgLaundry = new BangGiaCoDinh(dvLaundry.Id, "Bảng giá giặt ủi 2026", DateTimeOffset.Now, 20000);
        if (adminId != 0) bgLaundry.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgLaundry);

        // Ký hợp đồng với Cleany
        var hdLaundry = cleany.KyHopDongMoi("HD-CLEAN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 50000000, dvLaundry.Id, "Hợp đồng cung cấp dịch vụ giặt ủi định kỳ.");
        if (adminId != 0) hdLaundry.SetCreated(adminId, DateTimeOffset.Now);

        // 3.6. Dịch vụ giúp việc theo giờ
        var dvCleaning = new DichVu("DV_CLEANING", "Dịch vụ dọn dẹp", LoaiDichVu.TienIch, "Giờ", "Vệ sinh căn hộ theo yêu cầu.", null, false);
        dvCleaning.Activate();
        if (adminId != 0) dvCleaning.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvCleaning);
        await context.SaveChangesAsync();

        var bgCleaning = new BangGiaCoDinh(dvCleaning.Id, "Bảng giá dọn dẹp 2026", DateTimeOffset.Now, 100000);
        if (adminId != 0) bgCleaning.SetCreated(adminId, DateTimeOffset.Now);
        await context.BangGias.AddAsync(bgCleaning);

        // Ký hợp đồng với bTaskee
        var hdCleaning = btaskee.KyHopDongMoi("HD-BTASK-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 30000000, dvCleaning.Id, "Hợp đồng hợp tác cung cấp nhân sự giúp việc qua ứng dụng.");
        if (adminId != 0) hdCleaning.SetCreated(adminId, DateTimeOffset.Now);

        // --- 4. Dịch vụ Sửa chữa & Bảo trì (Thuê ngoài chuyên dụng) ---

        // 4.1. Đối tác Thang máy
        var schindler = new DoiTac("Schindler VN", "Công ty TNHH Schindler Việt Nam", "Jovan Vujovic", "GPKD-SCH-001", "MST-0301438914", "Số 2-4-6-8, Đường số 2, Tân Hưng, Quận 7, TP. HCM", "02837760900", "vietnam@schindler.com", "Đối tác bảo trì hệ thống thang máy khu dân cư.");
        if (adminId != 0) schindler.SetCreated(adminId, DateTimeOffset.Now);
        await context.DoiTacs.AddAsync(schindler);

        var dvThangMay = new DichVu("DV_BT_THANGMAY", "Bảo trì thang máy", LoaiDichVu.SuaChua, "Lần", "Dịch vụ bảo trì và sửa chữa thang máy định kỳ.", null, true);
        dvThangMay.Activate();
        if (adminId != 0) dvThangMay.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvThangMay);
        await context.SaveChangesAsync();

        var hdThangMay = schindler.KyHopDongMoi("HD-SCH-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 120000000, dvThangMay.Id, "Hợp đồng bảo trì hệ thống thang máy tòan tòa nhà.");
        if (adminId != 0) hdThangMay.SetCreated(adminId, DateTimeOffset.Now);

        // 4.2. Đối tác Điều hòa/Điện lạnh
        var daikin = new DoiTac("Daikin VN", "Công ty Cổ phần Daikin Air Conditioning (Vietnam)", "Kobatake Satoshi", "GPKD-DAIKIN-001", "MST-0305040153", "Lầu 12, Tòa nhà Viettel, 285 Cách Mạng Tháng Tám, Quận 10, TP. HCM", "18006777", "info@daikin.com.vn", "Đối tác bảo trì hệ thống điều hòa và thông gió.");
        if (adminId != 0) daikin.SetCreated(adminId, DateTimeOffset.Now);
        await context.DoiTacs.AddAsync(daikin);

        var dvDieuHoa = new DichVu("DV_SC_DIEUHOA", "Sửa chữa điều hòa", LoaiDichVu.SuaChua, "Máy", "Dịch vụ kiểm tra và sửa chữa điều hòa cho cư dân và khu vực chung.", null, false);
        dvDieuHoa.Activate();
        if (adminId != 0) dvDieuHoa.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvDieuHoa);
        await context.SaveChangesAsync();

        var hdDieuHoa = daikin.KyHopDongMoi("HD-DAIKIN-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), 50000000, dvDieuHoa.Id, "Hợp đồng sửa chữa và cung cấp linh kiện điều hòa.");
        if (adminId != 0) hdDieuHoa.SetCreated(adminId, DateTimeOffset.Now);

        // 4.3. Dịch vụ sửa chữa điện (Đối tác nguồn cung ứng - Thêm hợp đồng sửa chữa)
        var dvSuaDien = new DichVu("DV_SC_DIEN", "Sửa chữa hệ thống điện", LoaiDichVu.SuaChua, "Lần", "Dịch vụ kiểm tra khẩn cấp và khắc phục sự cố điện lưới.", null, false);
        dvSuaDien.Activate();
        if (adminId != 0) dvSuaDien.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvSuaDien);
        await context.SaveChangesAsync();

        var hdSuaDien = evn.KyHopDongMoi("HD-EVN-REPAIR-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2), 100000000, dvSuaDien.Id, "Hợp đồng cung cấp thợ kỹ thuật xử lý sự cố điện hộ dân.");
        if (adminId != 0) hdSuaDien.SetCreated(adminId, DateTimeOffset.Now);

        // 4.4. Dịch vụ sửa chữa nước (Đối tác nguồn cung ứng - Thêm hợp đồng sửa chữa)
        var dvSuaNuoc = new DichVu("DV_SC_NUOC", "Sửa chữa hệ thống nước", LoaiDichVu.SuaChua, "Lần", "Dịch vụ xử lý rò rỉ và thông tắc hệ thống cấp thoát nước.", null, false);
        dvSuaNuoc.Activate();
        if (adminId != 0) dvSuaNuoc.SetCreated(adminId, DateTimeOffset.Now);
        await context.DichVus.AddAsync(dvSuaNuoc);
        await context.SaveChangesAsync();

        var hdSuaNuoc = sawaco.KyHopDongMoi("HD-SW-REPAIR-2026", DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2), 80000000, dvSuaNuoc.Id, "Hợp đồng cung cấp thợ kỹ thuật xử lý sự cố cấp thoát nước.");
        if (adminId != 0) hdSuaNuoc.SetCreated(adminId, DateTimeOffset.Now);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Mandatory Services and Partners Seeded Successfully.");
    }
}
