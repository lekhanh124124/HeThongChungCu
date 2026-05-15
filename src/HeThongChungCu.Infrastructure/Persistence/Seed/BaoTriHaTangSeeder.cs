using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class BaoTriHaTangSeeder
{
    private static readonly string[] value =
            [
                "Kiểm tra hệ thống cáp kéo và ray dẫn hướng",
                "Kiểm tra thắng cơ và bộ khống chế vượt tốc",
                "Vệ sinh hố thang và phòng máy",
                "Kiểm tra tủ điều khiển và nguồn điện dự phòng",
                "Kiểm tra cảm biến và cơ cấu đóng mở cửa"
            ];
    private static readonly string[] valueArray =
            [
                "Kiểm tra đầu báo khói, đầu báo nhiệt",
                "Thử nghiệm còi báo động và đèn chớp",
                "Kiểm tra áp suất bình chữa cháy xách tay",
                "Kiểm tra tủ trung tâm báo cháy",
                "Thử nghiệm nút ấn khẩn cấp"
            ];
    private static readonly string[] valueArray0 =
            [
                "Kiểm tra lượng dầu diesel và nước làm mát",
                "Kiểm tra điện áp bình ắc quy đề",
                "Kiểm tra bộ sạc tự động",
                "Thử chạy không tải 15 phút",
                "Vệ sinh lọc gió và bề mặt máy"
            ];
    private static readonly string[] valueArray1 =
            [
                "Vệ sinh ống kính camera",
                "Kiểm tra nguồn cấp và đầu nối cáp",
                "Kiểm tra góc quan sát và tiêu cự",
                "Kiểm tra trạng thái ghi hình của đầu ghi",
                "Vệ sinh tủ rack chứa đầu ghi"
            ];
    private static readonly string[] valueArray2 =
            [
                "Kiểm tra độ rung và tiếng ồn của quạt",
                "Kiểm tra dòng điện động cơ quạt",
                "Vệ sinh cánh quạt và lưới lọc bụi",
                "Kiểm tra cảm biến nồng độ khí CO",
                "Kiểm tra tủ điện điều khiển quạt"
            ];
    private static readonly string[] valueArray3 =
            [
                "Kiểm tra mức dầu bôi trơn",
                "Kiểm tra rò rỉ khớp nối và đường ống",
                "Thử khởi động bơm bằng tay và tự động",
                "Kiểm tra đồng hồ đo áp suất",
                "Vệ sinh rọ hút và lọc rác"
            ];

    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.ThietBis.AnyAsync())
        {
            logger.LogInformation("Infrastructure Maintenance data already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding Infrastructure Maintenance (BaoTriHaTang)...");

        var faker = new Faker("vi");

        // 1. Lấy thông tin tài khoản admin để gán người tạo
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 1;

        // 2. Lấy thông tin nhân viên kỹ thuật và quản lý để phân công công việc
        var technicians = await context.NhanViens
            .AsNoTracking()
            .Where(n => n.LoaiNhanVienId == LoaiNhanVien.KyThuat)
            .Join(context.NguoiDung.AsNoTracking(),
                nv => nv.NguoiDungId,
                nd => nd.Id,
                (nv, nd) => new { NhanVienId = nv.Id, nd.Ho, nd.Ten, nd.CCCD, SoDienThoai = nd.SoDienThoai != null ? nd.SoDienThoai.Value : null })
            .ToListAsync();

        var manager = await context.NhanViens
            .Where(n => n.LoaiNhanVienId == LoaiNhanVien.QuanLy)
            .FirstOrDefaultAsync();
        var managerId = manager?.Id ?? 1;

        // 3. Lấy hợp đồng đối tác ngoài cho việc bảo trì thuê ngoài (Thang máy Schindler & Điều hòa Daikin)
        var schindlerContract = await context.HopDongDoiTacs.FirstOrDefaultAsync(h => h.SoHopDong == "HD-SCH-2026");
        var daikinContract = await context.HopDongDoiTacs.FirstOrDefaultAsync(h => h.SoHopDong == "HD-DAIKIN-2026");
        
        // 4. Lấy danh sách tòa nhà để gán sở hữu thiết bị
        var buildings = await context.ToaNhas.AsNoTracking().ToListAsync();
        var toaA = buildings.FirstOrDefault(b => b.Block == "A")?.Id;
        var toaB = buildings.FirstOrDefault(b => b.Block == "B")?.Id;
        var toaC = buildings.FirstOrDefault(b => b.Block == "C")?.Id;

        // --- 4. SEED THIẾT BỊ HẠ TẦNG (ThietBi) ---
        var thietBis = new List<ThietBi>
        {
            ThietBi.Create("TB-OT-A01", "Thang máy Otis Block A1", "Thang máy", "Sảnh Block A, Cột thang số 1", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 1200000000m, "Thang máy chở khách tốc độ cao.", toaA),
            ThietBi.Create("TB-OT-A02", "Thang máy Otis Block A2", "Thang máy", "Sảnh Block A, Cột thang số 2", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 1200000000m, "Thang máy chở hàng và băng ca cứu thương.", toaA),
            ThietBi.Create("TB-OT-B01", "Thang máy Otis Block B1", "Thang máy", "Sảnh Block B, Cột thang số 1", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 1200000000m, "Thang máy chở khách tốc độ cao.", toaB),
            ThietBi.Create("TB-HC-001", "Hệ thống PCCC tự động Hochiki", "Phòng cháy chữa cháy", "Phòng kỹ thuật PCCC, Tầng hầm B1", DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(2), 2500000000m, "Hệ thống trung tâm báo cháy tự động và vòi phun.", null),
            ThietBi.Create("TB-CM-500", "Máy phát điện dự phòng Cummins 500kVA", "Nguồn điện", "Phòng máy phát điện, Tầng hầm B2", DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(2), 1800000000m, "Hệ thống điện dự phòng tự động chuyển mạch ATS khi mất điện tổng.", null),
            ThietBi.Create("TB-CAM-H1", "Hệ thống camera giám sát Hikvision hầm xe", "An ninh giám sát", "Hầm xe B1 & B2", DateTimeOffset.Now.AddYears(-1), DateTimeOffset.Now.AddYears(1), 350000000m, "Gồm 80 camera IP hồng ngoại giám sát 24/7.", null),
            ThietBi.Create("TB-VT-A01", "Hệ thống thông gió tăng áp buồng thang Block A", "Thông gió điều hòa", "Buồng thang thoát hiểm Block A", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 450000000m, "Quạt tăng áp hút khói cưỡng bức khi có sự cố.", toaA),
            ThietBi.Create("TB-VT-B01", "Hệ thống thông gió tăng áp buồng thang Block B", "Thông gió điều hòa", "Buồng thang thoát hiểm Block B", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 450000000m, "Quạt tăng áp hút khói cưỡng bức khi có sự cố.", toaB),
            ThietBi.Create("TB-DK-VRV", "Hệ thống điều hòa trung tâm VRV Daikin sảnh chính", "Thông gió điều hòa", "Sảnh đón khách chính", DateTimeOffset.Now.AddYears(-1), DateTimeOffset.Now.AddYears(2), 850000000m, "Điều hòa trung tâm phục vụ sảnh và phòng cộng đồng.", null),
            ThietBi.Create("TB-PT-015", "Máy bơm nước sinh hoạt Pentax 15kW", "Cấp thoát nước", "Trạm bơm nước, Tầng hầm B2", DateTimeOffset.Now.AddYears(-2), DateTimeOffset.Now.AddYears(1), 150000000m, "Hệ thống bơm tăng áp cung cấp nước sạch lên bồn mái.", null),
            ThietBi.Create("TB-PT-022", "Máy bơm nước cứu hỏa Pentax 22kW", "Cấp thoát nước", "Trạm bơm cứu hỏa, Tầng hầm B1", DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(1), 220000000m, "Máy bơm chuyên dụng kết nối trực tiếp họng tiếp nước PCCC.", null),
            ThietBi.Create("TB-NT-001", "Hệ thống xử lý nước thải trung tâm", "Cấp thoát nước", "Khu kỹ thuật xử lý nước thải", DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(1), 1100000000m, "Hệ thống lọc sinh học khép kín bảo vệ môi trường.", null)
        };

        // Phân phối ngẫu nhiên trạng thái hoạt động thực tế của thiết bị hạ tầng
        thietBis[0].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[1].UpdateTrangThai(TrangThaiThietBi.CanBaoTri); // Cần bảo trì thang máy B1
        thietBis[2].UpdateTrangThai(TrangThaiThietBi.DangBaoTri); // Thang máy A2 đang bảo trì
        thietBis[3].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[4].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[5].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[6].UpdateTrangThai(TrangThaiThietBi.DangHong); // Hệ thống quạt thông gió Block A đang báo hỏng
        thietBis[7].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[8].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[9].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[10].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
        thietBis[11].UpdateTrangThai(TrangThaiThietBi.HoatDongTot);

        foreach (var tb in thietBis)
        {
            tb.SetCreated(adminId, DateTimeOffset.Now.AddMonths(-12));
        }
        await context.ThietBis.AddRangeAsync(thietBis);
        await context.SaveChangesAsync();

        // --- 5. SEED HẠNG MỤC BẢO TRÌ (HangMucBaoTri) ---
        var hmThangMay = HangMucBaoTri.Create(
            "HM-TM",
            "Bảo dưỡng thang máy định kỳ",
            "Kiểm tra toàn bộ hệ thống cáp kéo, ray dẫn hướng, thắng cơ và tủ điều khiển thang máy",
            120,
            1500000m,
            JsonSerializer.Serialize(value));

        var hmPccc = HangMucBaoTri.Create(
            "HM-PCCC",
            "Kiểm tra định kỳ hệ thống phòng cháy chữa cháy",
            "Thử nghiệm chức năng đầu báo khói, còi báo, tủ trung tâm và kiểm tra bình chữa cháy xách tay",
            180,
            2500000m,
            JsonSerializer.Serialize(valueArray));

        var hmMpd = HangMucBaoTri.Create(
            "HM-MPD",
            "Bảo dưỡng máy phát điện dự phòng",
            "Kiểm tra dầu diesel, nước làm mát, bình đề ắc quy và chạy thử tải không dòng",
            150,
            3000000m,
            JsonSerializer.Serialize(valueArray0));

        var hmCamera = HangMucBaoTri.Create(
            "HM-CAM",
            "Kiểm tra và vệ sinh hệ thống camera giám sát",
            "Lau chùi thấu kính camera, kiểm tra đường truyền tín hiệu nguồn và dữ liệu ổ cứng đầu ghi",
            90,
            800000m,
            JsonSerializer.Serialize(valueArray1));

        var hmThongGio = HangMucBaoTri.Create(
            "HM-TG",
            "Bảo dưỡng hệ thống thông gió hầm xe",
            "Đo độ rung động cơ, vệ sinh cánh quạt, kiểm tra tủ điện điều khiển và cảm biến khí CO",
            120,
            1200000m,
            JsonSerializer.Serialize(valueArray2));

        var hmMayBom = HangMucBaoTri.Create(
            "HM-BOM",
            "Bảo dưỡng máy bơm nước cứu hỏa",
            "Kiểm tra bôi trơn vòng bi, van một chiều, thử áp lực khởi động nhanh tự động",
            90,
            1000000m,
            JsonSerializer.Serialize(valueArray3));

        var hangMucs = new List<HangMucBaoTri> { hmThangMay, hmPccc, hmMpd, hmCamera, hmThongGio, hmMayBom };
        foreach (var hm in hangMucs)
        {
            hm.SetCreated(adminId, DateTimeOffset.Now.AddMonths(-12));
        }
        await context.HangMucBaoTris.AddRangeAsync(hangMucs);
        await context.SaveChangesAsync();

        // --- 6. SEED LỊCH BẢO TRÌ ĐỊNH KỲ (LichBaoTri) ---
        var lichBaoTris = new List<LichBaoTri>
        {
            // Thang máy Otis Block A1 -> Bảo dưỡng thang máy định kỳ hàng tháng
            LichBaoTri.Create(thietBis[0].Id, hmThangMay.Id, TanSuatBaoTri.HangThang, DateTimeOffset.Now.AddMonths(-6), null),
            // Thang máy Otis Block B1 -> Bảo dưỡng thang máy định kỳ hàng tháng
            LichBaoTri.Create(thietBis[2].Id, hmThangMay.Id, TanSuatBaoTri.HangThang, DateTimeOffset.Now.AddMonths(-6), null),
            // Hệ thống PCCC tự động Hochiki -> Kiểm tra định kỳ PCCC hàng quý
            LichBaoTri.Create(thietBis[3].Id, hmPccc.Id, TanSuatBaoTri.HangQuy, DateTimeOffset.Now.AddMonths(-9), null),
            // Máy phát điện Cummins -> Bảo dưỡng máy phát điện sau mỗi 6 tháng
            LichBaoTri.Create(thietBis[4].Id, hmMpd.Id, TanSuatBaoTri.SauThang, DateTimeOffset.Now.AddMonths(-12), null),
            // Hệ thống camera hầm xe -> Kiểm tra camera định kỳ hàng tháng
            LichBaoTri.Create(thietBis[5].Id, hmCamera.Id, TanSuatBaoTri.HangThang, DateTimeOffset.Now.AddMonths(-3), null),
            // Hệ thống quạt thông gió buồng tăng áp Block A -> Bảo dưỡng định kỳ hàng quý
            LichBaoTri.Create(thietBis[6].Id, hmThongGio.Id, TanSuatBaoTri.HangQuy, DateTimeOffset.Now.AddMonths(-6), null),
            // Máy bơm nước cứu hỏa -> Bảo dưỡng định kỳ hàng tháng
            LichBaoTri.Create(thietBis[10].Id, hmMayBom.Id, TanSuatBaoTri.HangThang, DateTimeOffset.Now.AddMonths(-6), null)
        };

        // Ghi nhận lịch sử chạy thực tế để máy tính ngày bảo trì tiếp theo khớp hoàn chỉnh
        lichBaoTris[0].RecordExecution(DateTimeOffset.Now.AddDays(-10)); // Mới chạy 10 ngày trước, tiếp theo sau 20 ngày nữa
        lichBaoTris[1].RecordExecution(DateTimeOffset.Now.AddDays(-15));
        lichBaoTris[2].RecordExecution(DateTimeOffset.Now.AddMonths(-1)); // Mới chạy 1 tháng trước, tiếp theo sau 2 tháng nữa
        lichBaoTris[3].RecordExecution(DateTimeOffset.Now.AddMonths(-2)); // Mới chạy 2 tháng trước, tiếp theo sau 4 tháng nữa
        lichBaoTris[4].RecordExecution(DateTimeOffset.Now.AddDays(-5));
        lichBaoTris[5].RecordExecution(DateTimeOffset.Now.AddMonths(-2).AddDays(-10));
        lichBaoTris[6].RecordExecution(DateTimeOffset.Now.AddDays(-12));

        foreach (var lich in lichBaoTris)
        {
            lich.SetCreated(adminId, DateTimeOffset.Now.AddMonths(-12));
        }
        await context.LichBaoTris.AddRangeAsync(lichBaoTris);
        await context.SaveChangesAsync();

        // --- 7. SEED PHIẾU BẢO TRÌ (PhieuBaoTri) & CHI TIẾT ĐI KÈM ---
        // Chúng ta sẽ khởi tạo danh sách phiếu bảo trì theo chu trình nghiệp vụ từng bước để tuân thủ 100% Business Logic
        var tickets = new List<PhieuBaoTri>();

        // Lấy danh sách checklist chuẩn từ hạng mục
        var checklistTM = JsonSerializer.Deserialize<string[]>(hmThangMay.ChecklistTieuChuan)!;
        var checklistPCCC = JsonSerializer.Deserialize<string[]>(hmPccc.ChecklistTieuChuan)!;
        var checklistMPD = JsonSerializer.Deserialize<string[]>(hmMpd.ChecklistTieuChuan)!;
        var checklistCAM = JsonSerializer.Deserialize<string[]>(hmCamera.ChecklistTieuChuan)!;
        var checklistTG = JsonSerializer.Deserialize<string[]>(hmThongGio.ChecklistTieuChuan)!;
        var checklistBOM = JsonSerializer.Deserialize<string[]>(hmMayBom.ChecklistTieuChuan)!;

        // Trạng thái 1: CHỜ GIAO VIỆC (ChoGiaoViec)
        // Phiếu được tạo sẵn từ lịch cho kế hoạch bảo trì sắp tới
        var ticket1 = PhieuBaoTri.Create("PBT-TM-01", thietBis[0].Id, hmThangMay.Id, lichBaoTris[0].Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(5), checklistTM);
        ticket1.SetCreated(adminId, DateTimeOffset.Now.AddDays(-1));
        tickets.Add(ticket1);

        var ticket2 = PhieuBaoTri.Create("PBT-CAM-01", thietBis[5].Id, hmCamera.Id, lichBaoTris[4].Id, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(7), checklistCAM);
        ticket2.SetCreated(adminId, DateTimeOffset.Now);
        tickets.Add(ticket2);

        // Trạng thái 2: ĐÃ GIAO VIỆC (DaGiaoViec)
        // Đã phân công nhân sự/đối tác nhưng chưa nhấn bắt đầu
        var ticket3 = PhieuBaoTri.Create("PBT-TM-02", thietBis[2].Id, hmThangMay.Id, lichBaoTris[1].Id, DateTimeOffset.Now.AddDays(-2), DateTimeOffset.Now.AddDays(3), checklistTM);
        ticket3.SetCreated(adminId, DateTimeOffset.Now.AddDays(-2));
        if (schindlerContract != null)
        {
            ticket3.AssignPartner(schindlerContract.Id);
        }
        else if (technicians.Count > 0)
        {
            var tech = technicians[0];
            ticket3.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Kỹ thuật viên chính", tech.NhanVienId)]);
        }
        tickets.Add(ticket3);

        var ticket4 = PhieuBaoTri.Create("PBT-PCCC-01", thietBis[3].Id, hmPccc.Id, lichBaoTris[2].Id, DateTimeOffset.Now.AddDays(-3), DateTimeOffset.Now.AddDays(2), checklistPCCC);
        ticket4.SetCreated(adminId, DateTimeOffset.Now.AddDays(-3));
        if (technicians.Count > 1)
        {
            var tech1 = technicians[0];
            var tech2 = technicians[1];
            ticket4.AssignStaff([
                NhanSuBaoTri.Create($"{tech1.Ho} {tech1.Ten}", tech1.CCCD ?? "079012345678", tech1.SoDienThoai ?? "0912345678", "Trưởng nhóm kỹ thuật", tech1.NhanVienId),
                NhanSuBaoTri.Create($"{tech2.Ho} {tech2.Ten}", tech2.CCCD ?? "079087654321", tech2.SoDienThoai ?? "0987654321", "Kỹ thuật viên phụ trợ", tech2.NhanVienId)
            ]);
        }
        tickets.Add(ticket4);

        // Trạng thái 3: ĐANG THỰC HIỆN (DangThucHien)
        // Kỹ thuật viên đã tiếp nhận tại hiện trường và đang thao tác
        var ticket5 = PhieuBaoTri.Create("PBT-MPD-01", thietBis[4].Id, hmMpd.Id, lichBaoTris[3].Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddHours(4), checklistMPD);
        ticket5.SetCreated(adminId, DateTimeOffset.Now.AddDays(-1));
        if (technicians.Count > 0)
        {
            var tech = technicians[faker.Random.Number(0, technicians.Count - 1)];
            ticket5.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Vận hành viên chính", tech.NhanVienId)]);
        }
        if (ticket5.TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaGiaoViec)
        {
            ticket5.Start();
        }
        tickets.Add(ticket5);

        // Trạng thái 6: ĐÃ HỦY (DaHuy)
        // Phiếu bị hủy kèm lý do trước khi nộp kết quả
        var ticket6 = PhieuBaoTri.Create("PBT-TG-01", thietBis[6].Id, hmThongGio.Id, lichBaoTris[5].Id, DateTimeOffset.Now.AddDays(-15), DateTimeOffset.Now.AddDays(-10), checklistTG);
        ticket6.SetCreated(adminId, DateTimeOffset.Now.AddDays(-15));
        ticket6.Cancel("Hủy phiếu do quạt đang hỏng nặng, chờ nhà sản xuất bảo hành thay mới toàn bộ động cơ chính.");
        tickets.Add(ticket6);

        // Lưu trước đợt 1 để gán ID tự tăng từ cơ sở dữ liệu cho PhieuBaoTri và PhieuBaoTriChecklist
        await context.PhieuBaoTris.AddRangeAsync(tickets);
        await context.SaveChangesAsync();

        // -------------------------------------------------------------
        // Trạng thái 4: CHỜ NGHIỆM THU (ChoNghiemThu)
        // Đã hoàn thành, nộp báo cáo kết quả và vật tư tiêu hao, đang đợi quản lý phê duyệt
        var ticket7 = PhieuBaoTri.Create("PBT-BOM-01", thietBis[10].Id, hmMayBom.Id, lichBaoTris[6].Id, DateTimeOffset.Now.AddDays(-4), DateTimeOffset.Now.AddDays(-2), checklistBOM);
        ticket7.SetCreated(adminId, DateTimeOffset.Now.AddDays(-4));
        if (technicians.Count > 0)
        {
            var tech = technicians[0];
            ticket7.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Kỹ thuật viên chính", tech.NhanVienId)]);
        }
        if (ticket7.TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaGiaoViec)
        {
            ticket7.Start();
        }

        // Lưu để có ID checklist
        await context.PhieuBaoTris.AddAsync(ticket7);
        await context.SaveChangesAsync();

        // Cập nhật kết quả checklist (Tất cả Đạt yêu cầu)
        var checklistUpdates7 = ticket7.Checklists.ToDictionary(
            c => c.Id,
            c => (DatYeuCau: true, GhiChu: (string?)"Hoạt động êm ái, thông số bình thường.", AnhId: (int?)null)
        );

        // Khai báo vật tư tiêu thụ thực tế
        var materials7 = new List<PhieuBaoTriVatTu>
        {
            PhieuBaoTriVatTu.Create("Dầu nhớt bôi trơn máy bơm Castrol", 2, 180000m),
            PhieuBaoTriVatTu.Create("Gioăng cao su chống rò rỉ DN100", 1, 95000m)
        };

        ticket7.SubmitResults(checklistUpdates7, materials7, 500000m, "Đã bảo trì hoàn tất hệ thống bơm cứu hỏa định kỳ. Máy bơm đã chạy thử áp lực ổn định, hệ thống không rò rỉ.");
        await context.SaveChangesAsync();


        // -------------------------------------------------------------
        // Trạng thái 5: ĐÃ HOÀN THÀNH (DaHoanThanh)
        // Đã qua toàn bộ luồng, được quản lý phê duyệt và ghi nhận chi phí, ngày nghiệm thu thực tế trong quá khứ.
        // Chúng ta sẽ tạo khoảng 10 phiếu hoàn thành trải dài trong 6 tháng qua để vẽ biểu đồ chi phí và thống kê hiệu năng.
        for (int m = 5; m >= 0; m--)
        {
            var baseDate = DateTimeOffset.Now.AddMonths(-m).AddDays(-faker.Random.Number(1, 25));

            // Thang máy (Đối tác Schindler bảo trì)
            var pbtTM = PhieuBaoTri.Create($"PBT-TM-DONE-0{m}", thietBis[m % 3].Id, hmThangMay.Id, null, baseDate.AddDays(-3), baseDate.AddDays(1), checklistTM);
            pbtTM.SetCreated(adminId, baseDate.AddDays(-3));
            if (schindlerContract != null)
            {
                pbtTM.AssignPartner(schindlerContract.Id);
            }
            else if (technicians.Count > 0)
            {
                var tech = technicians[faker.Random.Number(0, technicians.Count - 1)];
                pbtTM.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Kỹ thuật viên chính", tech.NhanVienId)]);
            }
            pbtTM.Start();

            // Lưu để lấy ID checklist
            await context.PhieuBaoTris.AddAsync(pbtTM);
            await context.SaveChangesAsync();

            var checklistDoneTM = pbtTM.Checklists.ToDictionary(
                c => c.Id,
                c => (DatYeuCau: true, GhiChu: (string?)"Đạt tiêu chuẩn vận hành", AnhId: (int?)null)
            );

            var vatTusTM = new List<PhieuBaoTriVatTu>
            {
                PhieuBaoTriVatTu.Create("Mỡ bôi trơn chuyên dụng ray dẫn hướng", 1, 220000m),
                PhieuBaoTriVatTu.Create("Dung dịch vệ sinh tiếp điểm tủ điện", 1, 135000m)
            };

            pbtTM.SubmitResults(checklistDoneTM, vatTusTM, 1500000m, "Hãng Schindler đã tiến hành bảo dưỡng đạt chuẩn an toàn quốc tế.");
            pbtTM.NghiemThu(managerId, baseDate.AddHours(2));
            pbtTM.SetModified(managerId, baseDate.AddHours(2));

            // ---------------------------------------------------------
            // Hệ thống PCCC (Nhân sự nội bộ xử lý)
            if (m % 2 == 0)
            {
                var pbtPCCC = PhieuBaoTri.Create($"PBT-PCCC-DONE-0{m}", thietBis[3].Id, hmPccc.Id, null, baseDate.AddDays(-2), baseDate, checklistPCCC);
                pbtPCCC.SetCreated(adminId, baseDate.AddDays(-2));
                if (technicians.Count > 0)
                {
                    var tech = technicians[faker.Random.Number(0, technicians.Count - 1)];
                    pbtPCCC.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Kỹ thuật viên chính", tech.NhanVienId)]);
                }
                if (pbtPCCC.TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaGiaoViec)
                {
                    pbtPCCC.Start();
                }

                await context.PhieuBaoTris.AddAsync(pbtPCCC);
                await context.SaveChangesAsync();

                // Giả định thỉnh thoảng có 1 mục bị lỗi nhẹ nhưng đã sửa chữa ngay tại chỗ
                var checklistDonePCCC = pbtPCCC.Checklists.Select((c, idx) => new { c.Id, Index = idx }).ToDictionary(
                    x => x.Id,
                    x => (
                        DatYeuCau: x.Index != 2,
                        GhiChu: x.Index == 2 ? (string?)"Phát hiện áp suất bình chữa cháy hành lang tầng 5 yếu, đã thay bình xách tay dự phòng mới." : (string?)"Đạt yêu cầu",
                        AnhId: (int?)null
                    )
                );

                var vatTusPCCC = new List<PhieuBaoTriVatTu>();
                if (m == 2)
                {
                    vatTusPCCC.Add(PhieuBaoTriVatTu.Create("Bột chữa cháy ABC 4kg nạp bổ sung", 4, 150000m));
                }

                pbtPCCC.SubmitResults(checklistDonePCCC, vatTusPCCC, 2500000m, "Hoàn tất kiểm tra thử nghiệm hệ thống PCCC, đèn còi báo khói hoạt động nhạy.");
                pbtPCCC.NghiemThu(managerId, baseDate.AddHours(4));
                pbtPCCC.SetModified(managerId, baseDate.AddHours(4));
            }

            // ---------------------------------------------------------
            // Hệ thống điều hòa Daikin (Đối tác Daikin bảo trì)
            if (m % 3 == 0)
            {
                var pbtDaikin = PhieuBaoTri.Create($"PBT-DK-DONE-0{m}", thietBis[8].Id, hmMayBom.Id, null, baseDate.AddDays(-1), baseDate, checklistBOM);
                pbtDaikin.SetCreated(adminId, baseDate.AddDays(-1));
                if (daikinContract != null)
                {
                    pbtDaikin.AssignPartner(daikinContract.Id);
                }
                else if (technicians.Count > 0)
                {
                    var tech = technicians[faker.Random.Number(0, technicians.Count - 1)];
                    pbtDaikin.AssignStaff([NhanSuBaoTri.Create($"{tech.Ho} {tech.Ten}", tech.CCCD ?? "079012345678", tech.SoDienThoai ?? "0912345678", "Kỹ thuật viên chính", tech.NhanVienId)]);
                }
                pbtDaikin.Start();

                await context.PhieuBaoTris.AddAsync(pbtDaikin);
                await context.SaveChangesAsync();

                var checklistDoneDK = pbtDaikin.Checklists.ToDictionary(
                    c => c.Id,
                    c => (DatYeuCau: true, GhiChu: (string?)"Dàn nóng giải nhiệt tốt, áp suất gas sạc đầy đủ.", AnhId: (int?)null)
                );

                var vatTusDK = new List<PhieuBaoTriVatTu>
                {
                    PhieuBaoTriVatTu.Create("Gas lạnh R410A bổ sung", 2, 350000m),
                    PhieuBaoTriVatTu.Create("Lưới lọc bụi sơ cấp dàn lạnh VRV", 2, 110000m)
                };

                pbtDaikin.SubmitResults(checklistDoneDK, vatTusDK, 1200000m, "Vệ sinh lưới lọc dàn lạnh, nạp gas bổ sung dàn nóng trung tâm sảnh chính.");
                pbtDaikin.NghiemThu(managerId, baseDate.AddHours(3));
                pbtDaikin.SetModified(managerId, baseDate.AddHours(3));
            }
        }

        // Dọn dẹp domain events trước khi kết thúc transaction
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully Seeded {DeviceCount} Devices, {CategoryCount} Categories, {ScheduleCount} Schedules and {TicketCount} Maintenance Orders.",
            thietBis.Count, hangMucs.Count, lichBaoTris.Count, await context.PhieuBaoTris.CountAsync());
    }
}
