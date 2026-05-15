using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

using System.Text.Json;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class HoaDonSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.HoaDons.AnyAsync())
        {
            logger.LogInformation("Invoices already seeded. Skipping HoaDonSeeder.");
            return;
        }

        logger.LogInformation("Seeding Invoices and Payments with comprehensive scenarios...");

        // 1. Lấy dữ liệu nền tảng
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 1;

        var specialEmails = new[] { "giangkiet2k4@gmail.com", "hongphat@gmail.com" };
        var specialUserAccounts = await context.TaiKhoan.IgnoreQueryFilters()
            .Where(t => specialEmails.Contains(t.Email.Value))
            .ToListAsync();
        var specialUserIds = specialUserAccounts
            .Where(a => a.NguoiDungId.HasValue)
            .Select(a => a.NguoiDungId!.Value)
            .ToList();

        if (!specialUserIds.Any())
        {
            logger.LogWarning("No special users found. Skipping invoice seeding.");
            return;
        }

        // Lấy các ID căn hộ đang cư trú hoạt động của special users
        var specialActiveApartmentIds = await context.QuanHeCuTrus
            .Where(r => specialUserIds.Contains(r.NguoiDungId) && 
                        r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru && 
                        (r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue))
            .Select(r => r.CanHoId)
            .Distinct()
            .ToListAsync();

        if (!specialActiveApartmentIds.Any())
        {
            logger.LogWarning("No active apartments found for special users. Skipping invoice seeding.");
            return;
        }

        // Lấy danh sách CanHo thực tế
        var specialActiveApartments = await context.CanHos
            .Where(c => specialActiveApartmentIds.Contains(c.Id))
            .ToListAsync();

        // Lấy danh sách dịch vụ phục vụ gán phí
        var dvQuanLy = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "MANAGEMENT_FEE");
        var dvDien = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "ELECTRICITY");
        var dvNuoc = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "WATER");
        var dvLaiTreHan = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "LATE_INTEREST_FEE");
        var dvMotor = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "PK_MOTOR");
        var dvCar = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "PK_CAR");
        var dvRac = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "DV_RAC");

        // 2. Tạo các Đợt thanh toán (DotThanhToan)
        var dotThang3 = DotThanhToan.Create("Đợt thanh toán 3/2026", new KyThanhToan(3, 2026), "Đợt thu phí định kỳ tháng 3 năm 2026").Value;
        dotThang3.MarkAsApproved();
        dotThang3.MarkAsIssued();

        var dotThang4 = DotThanhToan.Create("Đợt thanh toán 4/2026", new KyThanhToan(4, 2026), "Đợt thu phí định kỳ tháng 4 năm 2026").Value;
        dotThang4.MarkAsApproved();
        dotThang4.MarkAsIssued();

        var dotThang5 = DotThanhToan.Create("Đợt thanh toán 5/2026", new KyThanhToan(5, 2026), "Đợt thu phí định kỳ tháng 5 năm 2026").Value;
        // Đợt tháng 5 giữ ở trạng thái Tạo mới (Draft) để người dùng có thể duyệt thủ công

        await context.DotThanhToan.AddRangeAsync(dotThang3, dotThang4, dotThang5);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded 3 Billing periods (DotThanhToan): Month 3, 4, 5 of 2026.");

        // Lấy danh sách yêu cầu hoàn tất của căn hộ để gắn vào hóa đơn
        var listSuaChuaCompleted = await context.YeuCauSuaChuas
            .Where(s => s.TrangThaiId == TrangThaiYeuCau.Completed)
            .ToListAsync();

        var listThiCongChoCoc = await context.YeuCauThiCongs
            .Where(t => t.TrangThaiId == TrangThaiYeuCau.Approved)
            .ToListAsync();

        // 3. Seed hóa đơn và thanh toán cho Special Users
        int invoiceCodeCounter = 1000;
        int counterRelation = 0;
        foreach (var canHo in specialActiveApartments)
        {
            counterRelation++;
            var isOverdueScenario = counterRelation % 2 == 0;

            // Tìm chủ hộ tương ứng trong các special users
            var relationObj = await context.QuanHeCuTrus
                .FirstOrDefaultAsync(r => r.CanHoId == canHo.Id && specialUserIds.Contains(r.NguoiDungId));
            var nguoiDungId = relationObj != null ? relationObj.NguoiDungId : specialUserIds.First();

            // --- KỊCH BẢN THÁNG 3/2026 ---
            // Căn hộ 1 của specialUser sẽ: Thanh toán đầy đủ
            // Căn hộ 2 của specialUser sẽ: Quá hạn (Overdue) để tính dồn lãi chậm nộp sang kỳ tháng 5

            if (!isOverdueScenario)
            {
                // Kịch bản: Đã thanh toán (DaThanhToan)
                invoiceCodeCounter++;
                var maHD = $"HD-202603-{canHo.MaCanHo}";
                var ngayLap = new DateTimeOffset(2026, 3, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 3, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThang3.Id, maHD, new KyThanhToan(3, 2026), ngayLap, ngayHan, "Hóa đơn dịch vụ Tháng 3/2026");

                if (hdRes.IsSuccess)
                {
                    var hd = hdRes.Value;
                    // Gắn chi tiết phí
                    if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id, $"Tính theo diện tích {canHo.ThongSo.DienTich} m2");
                    if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 100, 250, 2167, dvDien.Id, "Tiêu thụ 150 kWh");
                    if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 50, 65, 7052, dvNuoc.Id, "Tiêu thụ 15 m3");
                    if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy (2 xe)", 2, 100000, dvMotor.Id, "Xe máy SH, Vision");
                    if (dvCar != null) hd.AddDichVuDetail("Phí giữ xe ô tô (1 xe)", 1, 1200000, dvCar.Id, "Xe ô tô Camry");
                    if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                    hd.UpdateStatus(TrangThaiHoaDon.DaThanhToan);
                    await context.HoaDons.AddAsync(hd);
                    // BỎ ClearAllDomainEvents để kích hoạt AutoRecordIncomeOnPaymentHandler
                    await context.SaveChangesAsync();

                    // Sinh giao dịch thành công cho từng chi tiết có phát sinh phí
                    foreach (var detail in hd.ChiTietHoaDons.Where(d => d.ThanhTien > 0))
                    {
                        var giaoDich = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT2603{detail.Id:D5}", "Thanh toán thành công qua Mobile Banking Techcombank").Value;
                        if (adminId != 0) giaoDich.SetCreated(adminId, ngayLap.AddDays(10));
                        await context.GiaoDichThanhToans.AddAsync(giaoDich);
                    }

                    // Lưu phiên giao dịch
                    var listIds = string.Join(",", hd.ChiTietHoaDons.Select(d => d.Id));
                    var phien = new PhienThanhToan($"TXN_3_{hd.Id}_1710931200", hd.Id, listIds, hd.TongTien, "Giao dịch thanh toán trọn gói hoàn thành");
                    phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                    phien.SetCreated(adminId, ngayLap.AddDays(10));
                    await context.PhienThanhToans.AddAsync(phien);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                // Kịch bản: Quá hạn (QuaHan)
                invoiceCodeCounter++;
                var maHD = $"HD-202603-{canHo.MaCanHo}";
                var ngayLap = new DateTimeOffset(2026, 3, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 3, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThang3.Id, maHD, new KyThanhToan(3, 2026), ngayLap, ngayHan, "Hóa đơn nợ đọng Tháng 3/2026");

                if (hdRes.IsSuccess)
                {
                    var hd = hdRes.Value;
                    if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id, $"Tính theo diện tích {canHo.ThongSo.DienTich} m2");
                    if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 50, 150, 1866, dvDien.Id, "Tiêu thụ 100 kWh");
                    if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 30, 42, 5973, dvNuoc.Id, "Tiêu thụ 12 m3");
                    if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);

                    hd.UpdateStatus(TrangThaiHoaDon.QuaHan);
                    hd.SetNgayTinhLai(DateTimeOffset.Now); // Đã bị quét tính lãi
                    await context.HoaDons.AddAsync(hd);
                }
            }

            // --- KỊCH BẢN THÁNG 4/2026 ---
            // Căn hộ 1: Thanh toán một phần (ThanhToanMotPhan) + có phí sửa chữa kèm theo
            // Căn hộ 2: Đã hủy (DaHuy) + Hóa đơn sửa đổi Đã thanh toán
            if (!isOverdueScenario)
            {
                invoiceCodeCounter++;
                var maHD = $"HD-202604-{canHo.MaCanHo}";
                var ngayLap = new DateTimeOffset(2026, 4, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 4, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThang4.Id, maHD, new KyThanhToan(4, 2026), ngayLap, ngayHan, "Hóa đơn dịch vụ tháng 4");

                if (hdRes.IsSuccess)
                {
                    var hd = hdRes.Value;
                    if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id);
                    if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 250, 480, 2729, dvDien.Id, "Tiêu thụ 230 kWh");
                    if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 65, 83, 8669, dvNuoc.Id, "Tiêu thụ 18 m3");
                    if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy (2 xe)", 2, 100000, dvMotor.Id);
                    if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                    // Tìm yêu cầu sửa chữa đã hoàn tất có phát sinh chi phí của căn hộ này để liên kết phí sửa chữa
                    var suaChua = listSuaChuaCompleted.FirstOrDefault(s => s.CanHoId == canHo.Id && s.ChiPhiThucTe > 0);
                    if (suaChua != null)
                    {
                        hd.AddSuaChuaDetail(suaChua.Id, $"Phí sửa chữa thực tế - {suaChua.NoiDung}", suaChua.ChiPhiThucTe ?? 120000);
                    }
                    else
                    {
                        // Hoặc tự mock một yêu cầu sửa chữa khẩn cấp trên database
                        var mockSuaChua = YeuCauSuaChua.Create(canHo.Id, PhamViSuaChua.TrongCanHo, LoaiSuCoKyThuat.Nuoc, "Sửa chữa rò rỉ van khóa nước bồn rửa mặt", "Nhà vệ sinh chính");
                        mockSuaChua.SetCreated(nguoiDungId, ngayLap.AddDays(-15));
                        mockSuaChua.Approve(adminId, ngayLap.AddDays(-14));
                        var techStaffId = await context.NhanViens.Select(nv => nv.Id).FirstOrDefaultAsync();
                        if (techStaffId != 0)
                        {
                            mockSuaChua.AssignInternalStaff(new[] { techStaffId });
                        }
                        else
                        {
                            mockSuaChua.AssignInternalStaff(new[] { 1 });
                        }
                        mockSuaChua.NhapBaoGia(150000, false, "Xác nhận báo giá qua điện thoại");
                        mockSuaChua.HoanTatXuLy(adminId, "Thay thế gioăng cao su khóa nước.", 150000, ngayLap.AddDays(-13));
                        await context.YeuCauSuaChuas.AddAsync(mockSuaChua);
                        DatabaseSeeder.ClearAllDomainEvents(context);
                        await context.SaveChangesAsync();
                        hd.AddSuaChuaDetail(mockSuaChua.Id, "Phí sửa chữa rò rỉ nước", 150000);
                    }

                    // Chuyển sang thanh toán một phần (đã thanh toán điện nước, nợ phí quản lý và sửa chữa)
                    hd.UpdateStatus(TrangThaiHoaDon.ThanhToanMotPhan);
                    await context.HoaDons.AddAsync(hd);
                    // BỎ ClearAllDomainEvents để kích hoạt AutoRecordIncomeOnPaymentHandler
                    await context.SaveChangesAsync();

                    // Chỉ sinh giao dịch cho điện & nước có phát sinh phí
                    var paidDetails = hd.ChiTietHoaDons.Where(d => (d.TenMucPhi.ToLower().Contains("điện") || d.TenMucPhi.ToLower().Contains("nước")) && d.ThanhTien > 0).ToList();
                    foreach (var detail in paidDetails)
                    {
                        var gd = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT2604{detail.Id:D5}", "Thanh toán từng phần tiền điện nước").Value;
                        gd.SetCreated(adminId, ngayLap.AddDays(10));
                        await context.GiaoDichThanhToans.AddAsync(gd);
                    }

                    var paidIds = string.Join(",", paidDetails.Select(d => d.Id));
                    var phien = new PhienThanhToan($"TXN_4_{hd.Id}_1713571200", hd.Id, paidIds, paidDetails.Sum(d => d.ThanhTien), "Đã thanh toán một phần hóa đơn (điện & nước)");
                    phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                    phien.SetCreated(adminId, ngayLap.AddDays(10));
                    await context.PhienThanhToans.AddAsync(phien);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                // Kịch bản: Hóa đơn bị hủy (DaHuy)
                invoiceCodeCounter++;
                var maHDHuy = $"HD-202604-{canHo.MaCanHo}-HUY";
                var ngayLap = new DateTimeOffset(2026, 4, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 4, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdHuyRes = HoaDon.CreateHoaDon(canHo.Id, dotThang4.Id, maHDHuy, new KyThanhToan(4, 2026), ngayLap, ngayHan, "Hóa đơn lỗi chỉ số nước");

                if (hdHuyRes.IsSuccess)
                {
                    var hdHuy = hdHuyRes.Value;
                    if (dvQuanLy != null) hdHuy.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id);
                    if (dvNuoc != null) hdHuy.AddTieuThuDetail("Tiền nước sạch sinh hoạt (Lỗi)", 30, 230, 15929, dvNuoc.Id, "Ghi nhầm 200 m3");
                    hdHuy.Cancel("Ghi nhận sai lệch chỉ số nước vượt định mức thực tế từ cư dân.");
                    await context.HoaDons.AddAsync(hdHuy);
                }

                // Hóa đơn lập mới thay thế (DaThanhToan)
                var maHDMoi = $"HD-202604-{canHo.MaCanHo}";
                var hdMoiRes = HoaDon.CreateHoaDon(canHo.Id, dotThang4.Id, maHDMoi, new KyThanhToan(4, 2026), ngayLap.AddDays(3), ngayHan, "Hóa đơn thay thế hóa đơn cũ bị hủy");

                if (hdMoiRes.IsSuccess)
                {
                    var hdMoi = hdMoiRes.Value;
                    if (dvQuanLy != null) hdMoi.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id);
                    if (dvNuoc != null) hdMoi.AddTieuThuDetail("Tiền nước sạch sinh hoạt (Đúng)", 30, 50, 7052, dvNuoc.Id, "Tiêu thụ chuẩn xác 20 m3");
                    if (dvMotor != null) hdMoi.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);

                    hdMoi.UpdateStatus(TrangThaiHoaDon.DaThanhToan);
                    await context.HoaDons.AddAsync(hdMoi);
                    // BỎ ClearAllDomainEvents để kích hoạt AutoRecordIncomeOnPaymentHandler
                    await context.SaveChangesAsync();

                    foreach (var detail in hdMoi.ChiTietHoaDons.Where(d => d.ThanhTien > 0))
                    {
                        var gd = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT2604R{detail.Id:D5}", "Thanh toán đầy đủ hóa đơn thay thế").Value;
                        gd.SetCreated(adminId, ngayLap.AddDays(12));
                        await context.GiaoDichThanhToans.AddAsync(gd);
                    }

                    var listIds = string.Join(",", hdMoi.ChiTietHoaDons.Select(d => d.Id));
                    var phien = new PhienThanhToan($"TXN_4_R_{hdMoi.Id}_1713571200", hdMoi.Id, listIds, hdMoi.TongTien, "Giao dịch thanh toán trọn gói thay thế hoàn tất");
                    phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                    phien.SetCreated(adminId, ngayLap.AddDays(12));
                    await context.PhienThanhToans.AddAsync(phien);
                    await context.SaveChangesAsync();
                }
            }

            // --- KỊCH BẢN THÁNG 5/2026 (HIỆN TẠI) ---
            // Căn hộ 1: Chưa thanh toán (ChuaThanhToan) + Có tiền cọc thi công + Đã sinh mã VietQR sẵn sàng
            // Căn hộ 2: Chờ duyệt (ChoDuyet) để Demo phê duyệt trên Web
            // Đồng thời dồn Lãi phạt chậm nộp nếu có nợ cũ từ Tháng 3
            if (!isOverdueScenario)
            {
                invoiceCodeCounter++;
                var maHD = $"HD-202605-{canHo.MaCanHo}";
                var ngayLap = new DateTimeOffset(2026, 5, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 5, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThang5.Id, maHD, new KyThanhToan(5, 2026), ngayLap, ngayHan, "Hóa đơn dịch vụ tháng này");

                if (hdRes.IsSuccess)
                {
                    var hd = hdRes.Value;
                    if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id);
                    if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 480, 690, 2729, dvDien.Id, "Tiêu thụ 210 kWh");
                    if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 83, 99, 7052, dvNuoc.Id, "Tiêu thụ 16 m3");
                    if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy (2 xe)", 2, 100000, dvMotor.Id);

                    // Tìm yêu cầu thi công để gán tiền đặt cọc thi công nội thất
                    var thiCong = listThiCongChoCoc.FirstOrDefault(t => t.CanHoId == canHo.Id);
                    if (thiCong != null)
                    {
                        hd.AddThiCongDetail(thiCong.Id, LoaiChiPhiThiCong.DatCoc, $"Tiền ký quỹ đặt cọc - {thiCong.HangMucThiCong}", thiCong.TienDatCoc ?? 10000000);
                        // Đánh dấu yêu cầu thi công đã được gán hóa đơn
                        thiCong.MarkAsBilled(hd.Id);
                    }
                    else
                    {
                        // Hoặc tự mock một yêu cầu thi công để gán cọc
                        var mockThiCong = YeuCauThiCong.Create(
                            canHo.Id,
                            "Thi công lắp đặt rèm cửa và tủ gỗ âm tường",
                            DateTimeOffset.Now,
                            DateTimeOffset.Now.AddDays(15),
                            "Thi công nội thất phòng ngủ",
                            "Nội Thất Xinh",
                            "Trần Văn Hùng",
                            "0988776655"
                        );
                        mockThiCong.SetCreated(nguoiDungId, ngayLap.AddDays(-5));
                        mockThiCong.SetTienDatCoc(5000000);
                        mockThiCong.AddTep(new TepYeuCauThiCong("ban-ve-chi-tiet.pdf", "path/file.pdf", 2500000, "application/pdf"));
                        mockThiCong.AddNhanSu("Nguyễn Văn A", "079095123456", "0981234567", "Thợ chính", "Lắp đặt rèm");
                        mockThiCong.Approve(adminId, ngayLap.AddDays(-4));
                        await context.YeuCauThiCongs.AddAsync(mockThiCong);
                        DatabaseSeeder.ClearAllDomainEvents(context);
                        await context.SaveChangesAsync();

                        hd.AddThiCongDetail(mockThiCong.Id, LoaiChiPhiThiCong.DatCoc, "Ký quỹ đặt cọc thi công nội thất", 5000000);
                        mockThiCong.MarkAsBilled(hd.Id);
                    }

                    // Chuyển sang Chưa thanh toán để cư dân thấy
                    hd.UpdateStatus(TrangThaiHoaDon.ChuaThanhToan);
                    await context.HoaDons.AddAsync(hd);
                    DatabaseSeeder.ClearAllDomainEvents(context);
                    await context.SaveChangesAsync();

                    // Khởi tạo một Phiên thanh toán đang chờ (ChoThanhToan) có VietQR Url cực chuyên nghiệp
                    var vietQrUrl = $"https://img.vietqr.io/image/TCB-19039590589017-print.png?amount={(int)hd.TongTien}&addInfo=TXN_5_{hd.Id}_1715012345&accountName=LE%20MINH%20KHANH";
                    var phien = new PhienThanhToan($"TXN_5_{hd.Id}_1715012345", hd.Id, "", hd.TongTien, "Gửi liên kết chuyển khoản QR ngân hàng");
                    phien.UpdateStatus(TrangThaiThanhToan.ChoThanhToan);
                    phien.SetCreated(nguoiDungId, ngayLap.AddDays(2));
                    
                    await context.PhienThanhToans.AddAsync(phien);
                }
            }
            else
            {
                // Kịch bản: Hóa đơn nháp / Chờ duyệt (ChoDuyet)
                invoiceCodeCounter++;
                var maHD = $"HD-202605-{canHo.MaCanHo}-DRAFT";
                var ngayLap = new DateTimeOffset(2026, 5, 5, 8, 0, 0, TimeSpan.FromHours(7));
                var ngayHan = new DateTimeOffset(2026, 5, 20, 23, 59, 59, TimeSpan.FromHours(7));
                var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThang5.Id, maHD, new KyThanhToan(5, 2026), ngayLap, ngayHan, "Hóa đơn dự thảo (Đang thẩm định)");

                if (hdRes.IsSuccess)
                {
                    var hd = hdRes.Value;
                    if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id);
                    if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 150, 310, 2167, dvDien.Id, "Tiêu thụ 160 kWh");
                    if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 42, 53, 7052, dvNuoc.Id, "Tiêu thụ 11 m3");
                    if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);

                    // THÊM: Cộng dồn Lãi phạt chậm nộp do nợ quá hạn từ tháng 3 (OverdueInvoice)
                    // Lãi phạt = 1,500,000 VND nợ gốc * 50 ngày trễ hạn * 0.05%/ngày
                    if (dvLaiTreHan != null)
                    {
                        hd.AddDichVuDetail($"Lãi phạt chậm nộp hóa đơn HD-202603-{canHo.MaCanHo}", 1, 37500, dvLaiTreHan.Id, "Trễ nộp 50 ngày tính từ ngày 20/03/2026");
                    }

                    // Giữ nguyên trạng thái ChoDuyet
                    await context.HoaDons.AddAsync(hd);
                }
            }
        }

        // 4. Seed thêm hóa đơn đơn giản cho 8 căn hộ ngẫu nhiên khác để đồ thị không bị trống trải và đạt số lượng kế hoạch
        var randomApartments = await context.CanHos
            .Where(c => !specialActiveApartmentIds.Contains(c.Id))
            .OrderBy(c => c.Id)
            .Take(8)
            .ToListAsync();

        int randomAptCounter = 0;
        foreach (var apt in randomApartments)
        {
            randomAptCounter++;
            bool isMonth4Paid = randomAptCounter <= 4; // 4 căn hộ đầu sẽ thanh toán Tháng 4, 4 căn hộ sau chưa thanh toán Tháng 4

            // --- THÁNG 3/2026 (ĐÃ THANH TOÁN CHO CẢ 8 CĂN HỘ) ---
            var maHD3 = $"HD-202603-{apt.MaCanHo}";
            var hd3Res = HoaDon.CreateHoaDon(apt.Id, dotThang3.Id, maHD3, new KyThanhToan(3, 2026), new DateTimeOffset(2026, 3, 5, 8, 0, 0, TimeSpan.FromHours(7)), new DateTimeOffset(2026, 3, 20, 23, 59, 59, TimeSpan.FromHours(7)), "Hóa đơn định kỳ");
            if (hd3Res.IsSuccess)
            {
                var hd = hd3Res.Value;
                if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, apt.ThongSo.DienTich * 10000, dvQuanLy.Id);
                if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 80, 200, 2167, dvDien.Id, "120 kWh");
                if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 35, 47, 5973, dvNuoc.Id, "12 m3");
                if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);
                if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                hd.UpdateStatus(TrangThaiHoaDon.DaThanhToan);
                await context.HoaDons.AddAsync(hd);
                // DatabaseSeeder.ClearAllDomainEvents(context); 
                await context.SaveChangesAsync();

                foreach (var detail in hd.ChiTietHoaDons.Where(d => d.ThanhTien > 0))
                {
                    var gd = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT2603R{detail.Id:D5}", "Thanh toán dịch vụ").Value;
                    if (adminId != 0) gd.SetCreated(adminId, new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.FromHours(7)));
                    await context.GiaoDichThanhToans.AddAsync(gd);
                }

                var listIds = string.Join(",", hd.ChiTietHoaDons.Select(d => d.Id));
                var phien = new PhienThanhToan($"TXN_3_R_{hd.Id}_1710931200", hd.Id, listIds, hd.TongTien, "Giao dịch thanh toán trọn gói hoàn thành");
                phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                if (adminId != 0) phien.SetCreated(adminId, new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.FromHours(7)));
                await context.PhienThanhToans.AddAsync(phien);
                await context.SaveChangesAsync();
            }

            // --- THÁNG 4/2026 ---
            var maHD4 = $"HD-202604-{apt.MaCanHo}";
            var hd4Res = HoaDon.CreateHoaDon(apt.Id, dotThang4.Id, maHD4, new KyThanhToan(4, 2026), new DateTimeOffset(2026, 4, 5, 8, 0, 0, TimeSpan.FromHours(7)), new DateTimeOffset(2026, 4, 20, 23, 59, 59, TimeSpan.FromHours(7)), "Hóa đơn định kỳ");
            if (hd4Res.IsSuccess)
            {
                var hd = hd4Res.Value;
                if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, apt.ThongSo.DienTich * 10000, dvQuanLy.Id);
                if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 200, 310, 2167, dvDien.Id, "110 kWh");
                if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 47, 58, 5973, dvNuoc.Id, "11 m3");
                if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);
                if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                if (isMonth4Paid)
                {
                    hd.UpdateStatus(TrangThaiHoaDon.DaThanhToan);
                }
                else
                {
                    hd.UpdateStatus(TrangThaiHoaDon.ChuaThanhToan);
                }

                await context.HoaDons.AddAsync(hd);
                // DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();

                if (isMonth4Paid)
                {
                    foreach (var detail in hd.ChiTietHoaDons.Where(d => d.ThanhTien > 0))
                    {
                        var gd = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT2604R{detail.Id:D5}", "Thanh toán dịch vụ").Value;
                        if (adminId != 0) gd.SetCreated(adminId, new DateTimeOffset(2026, 4, 15, 10, 0, 0, TimeSpan.FromHours(7)));
                        await context.GiaoDichThanhToans.AddAsync(gd);
                    }

                    var listIds = string.Join(",", hd.ChiTietHoaDons.Select(d => d.Id));
                    var phien = new PhienThanhToan($"TXN_4_R_{hd.Id}_1713571200", hd.Id, listIds, hd.TongTien, "Giao dịch thanh toán trọn gói hoàn thành");
                    phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                    if (adminId != 0) phien.SetCreated(adminId, new DateTimeOffset(2026, 4, 15, 10, 0, 0, TimeSpan.FromHours(7)));
                    await context.PhienThanhToans.AddAsync(phien);
                    await context.SaveChangesAsync();
                }
            }

            // --- THÁNG 5/2026 (CHƯA THANH TOÁN CHO CẢ 12 CĂN HỘ) ---
            var maHD5 = $"HD-202605-{apt.MaCanHo}";
            var hd5Res = HoaDon.CreateHoaDon(apt.Id, dotThang5.Id, maHD5, new KyThanhToan(5, 2026), new DateTimeOffset(2026, 5, 5, 8, 0, 0, TimeSpan.FromHours(7)), new DateTimeOffset(2026, 5, 20, 23, 59, 59, TimeSpan.FromHours(7)), "Hóa đơn định kỳ");
            if (hd5Res.IsSuccess)
            {
                var hd = hd5Res.Value;
                if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, apt.ThongSo.DienTich * 10000, dvQuanLy.Id);
                if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", 310, 440, 2167, dvDien.Id, "130 kWh");
                if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", 58, 70, 7052, dvNuoc.Id, "12 m3");
                if (dvMotor != null) hd.AddDichVuDetail("Phí giữ xe máy", 1, 100000, dvMotor.Id);
                if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                hd.UpdateStatus(TrangThaiHoaDon.ChuaThanhToan);
                await context.HoaDons.AddAsync(hd);
            }
        }

        // 5. Tự động sinh ChiSoTieuThu khớp với các chi tiết hóa đơn tiêu thụ đã seed
        var createdHoaDons = await context.HoaDons
            .Include(h => h.ChiTietHoaDons)
            .ToListAsync();

        var seededKeys = new HashSet<(int CanHoId, int DichVuId, int Thang, int Nam)>();

        // Ưu tiên hóa đơn không bị hủy lên trước, hóa đơn bị hủy xuống sau
        var orderedHoaDons = createdHoaDons
            .OrderBy(h => h.TrangThaiHoaDonId == TrangThaiHoaDon.DaHuy ? 1 : 0)
            .ToList();

        foreach (var hd in orderedHoaDons)
        {
            foreach (var detail in hd.ChiTietHoaDons)
            {
                if (detail is ChiTietHoaDonTieuThu tieuThuDetail)
                {
                    var key = (hd.CanHoId, tieuThuDetail.DichVuId, hd.KyThanhToan.Thang, hd.KyThanhToan.Nam);
                    if (seededKeys.Contains(key))
                    {
                        continue; // Đã seed cho cặp này rồi, bỏ qua để tránh trùng lặp
                    }

                    var cs = ChiSoTieuThu.Create(
                        hd.CanHoId,
                        tieuThuDetail.DichVuId,
                        tieuThuDetail.ChiSoCu,
                        tieuThuDetail.ChiSoMoi,
                        hd.KyThanhToan.Thang,
                        hd.KyThanhToan.Nam,
                        hd.NgayLap,
                        null,
                        tieuThuDetail.GhiChu,
                        null);

                    cs.Confirm();
                    cs.MarkAsBilled(hd.Id);

                    await context.ChiSoTieuThus.AddAsync(cs);
                    seededKeys.Add(key);
                }
            }
        }

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Database Seeding for Invoices and Payments completed successfully!");
    }
}
