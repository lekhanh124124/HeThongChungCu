using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class PhanAnhSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.YeuCauPhanAnhs.AnyAsync())
        {
            logger.LogInformation("Complaints (PhanAnh) already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding Complaints (PhanAnh) with comprehensive scenarios...");

        // 1. Get admin ID
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 1;

        // 2. Fetch special users and their active apartments
        var specialEmails = new[] { "giangkiet2k4@gmail.com", "hongphat@gmail.com" };
        var specialUserAccounts = await context.TaiKhoan.IgnoreQueryFilters()
            .Where(t => specialEmails.Contains(t.Email.Value))
            .ToListAsync();
        var specialUserIds = specialUserAccounts
            .Where(a => a.NguoiDungId.HasValue)
            .Select(a => a.NguoiDungId!.Value)
            .ToList();

        var relations = await context.QuanHeCuTrus
            .Where(r => specialUserIds.Contains(r.NguoiDungId) && 
                        (r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue))
            .ToListAsync();

        if (!relations.Any())
        {
            logger.LogWarning("No special user relations found for complaints seeding. Falling back to general residents.");
            relations = await context.QuanHeCuTrus
                .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue)
                .Take(5)
                .ToListAsync();
        }

        if (!relations.Any())
        {
            logger.LogWarning("No residents/apartments found in system. Skipping PhanAnhSeeder.");
            return;
        }

        // Map users to relations
        var gKietRelation = relations.FirstOrDefault(r => {
            var acc = specialUserAccounts.FirstOrDefault(a => a.NguoiDungId == r.NguoiDungId);
            return acc?.Email.Value == "giangkiet2k4@gmail.com";
        }) ?? relations.First();

        var hPhatRelation = relations.FirstOrDefault(r => {
            var acc = specialUserAccounts.FirstOrDefault(a => a.NguoiDungId == r.NguoiDungId);
            return acc?.Email.Value == "hongphat@gmail.com";
        }) ?? (relations.Count > 1 ? relations[1] : relations.First());

        var listPhanAnhs = new List<YeuCauPhanAnh>();

        // --- CASE 1: DaDong (Completed & Closed) - Giang Kiet (VeSinhMoitruong) ---
        var pa1 = YeuCauPhanAnh.Create(
            gKietRelation.CanHoId,
            "Hành lang tầng 12 đọng nước bẩn và bốc mùi",
            "Sáng nay đi làm thấy sàn hành lang tầng 12 Block A bị đọng vũng nước bẩn từ túi rác nhà ai đó rò rỉ ra, bốc mùi hôi rất khó chịu.",
            LoaiPhanAnh.VeSinhMoitruong,
            new[] { new TepYeuCauPhanAnh("vung_nuoc_hanh_lang.jpg", "https://example.com/uploads/vung_nuoc_hanh_lang.jpg", 152400, "image/jpeg") },
            true).Value;

        pa1.SetCreated(gKietRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-15));
        pa1.SetHanPhanHoi(pa1.CreatedAt.AddHours(pa1.LoaiPhanAnhId.HanXuLyGio));
        pa1.TiepNhanVaPhanCong(adminId, DateTimeOffset.Now.AddDays(-14));
        pa1.ThemPhanHoi("BQL đã nhận thông tin và cử nhân viên lao công lên xử lý dọn dẹp ngay lập tức.", true);
        pa1.ThemPhanHoi("Cảm ơn BQL, hiện tại hành lang đã sạch sẽ và thơm tho trở lại.", false);
        pa1.XacNhanHoanThanh(adminId, "Lao công đã lau dọn sạch, xịt khử mùi hành lang tầng 12.", DateTimeOffset.Now.AddDays(-14));
        pa1.CuDanDanhGiaVaDongTicket(5, "Xử lý rất nhanh chóng và triệt để, nhân viên vệ sinh thân thiện.");
        listPhanAnhs.Add(pa1);


        // --- CASE 2: DaDong (Completed & Closed) - Hong Phat (TaiChinhPhiDichVu) [Cost Source: Water Consumption] ---
        var pa2 = YeuCauPhanAnh.Create(
            hPhatRelation.CanHoId,
            "Thắc mắc về chỉ số nước sinh hoạt tháng 4 tăng đột biến",
            "Hóa đơn nước tháng 4 của nhà tôi báo tiêu thụ 50m3 nước, trong khi bình thường chỉ dùng khoảng 15-20m3. Mong BQL kiểm tra lại xem có bị ghi nhầm hay rò rỉ đường ống.",
            LoaiPhanAnh.TaiChinhPhiDichVu,
            new[] { new TepYeuCauPhanAnh("hoa_don_nuoc_t4.pdf", "https://example.com/uploads/hoa_don_nuoc_t4.pdf", 320400, "application/pdf") },
            true).Value;

        pa2.SetCreated(hPhatRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-10));
        pa2.SetHanPhanHoi(pa2.CreatedAt.AddHours(pa2.LoaiPhanAnhId.HanXuLyGio));
        pa2.TiepNhanVaPhanCong(adminId, DateTimeOffset.Now.AddDays(-9));
        pa2.ThemPhanHoi("BQL đã tiếp nhận phản ánh. Chúng tôi sẽ cử kỹ thuật xuống kiểm tra đồng hồ nước và đường ống cấp nước vào căn hộ của anh vào chiều nay.", true);
        pa2.ThemPhanHoi("[KẾT QUẢ KIỂM TRA]: Kỹ thuật đã kiểm tra đồng hồ hoạt động bình thường, tuy nhiên phát hiện phao bồn cầu nhà vệ sinh bị kẹt gây chảy nước liên tục. Kỹ thuật đã hỗ trợ căn chỉnh lại phao giúp anh. Số m3 nước trên đồng hồ là chính xác theo lượng nước rò rỉ thực tế.", true);
        pa2.ThemPhanHoi("À dạ, hèn chi tôi nghe tiếng nước chảy róc rách trong WC. Cảm ơn kỹ thuật BQL đã hỗ trợ tìm ra nguyên nhân và khắc phục giúp.", false);
        pa2.XacNhanHoanThanh(adminId, "Đã chỉnh lại phao bồn cầu chống rò nước, giải thích rõ nguyên nhân cho cư dân.", DateTimeOffset.Now.AddDays(-8));
        pa2.CuDanDanhGiaVaDongTicket(5, "Nhân viên kỹ thuật cực kỳ nhiệt tình, tìm ra lỗi nhanh chóng.");
        listPhanAnhs.Add(pa2);


        // --- CASE 3: ChoTiepNhan (Pending) - Giang Kiet (AnNinhBaoVe) ---
        var pa3 = YeuCauPhanAnh.Create(
            gKietRelation.CanHoId,
            "Xe máy đỗ sai vị trí chặn lối đi hầm xe B1",
            "Khu vực sảnh thang Block B hầm xe B1 có chiếc xe máy SH biển số 59-X1 123.45 đỗ chắn ngang lối vào thang máy, gây khó khăn cho cư dân di chuyển xe nôi và xe lăn.",
            LoaiPhanAnh.AnNinhBaoVe,
            new[] { new TepYeuCauPhanAnh("xe_do_chan_loi.jpg", "https://example.com/uploads/xe_do_chan_loi.jpg", 180300, "image/jpeg") },
            true).Value;

        pa3.SetCreated(gKietRelation.NguoiDungId, DateTimeOffset.Now.AddHours(-2));
        pa3.SetHanPhanHoi(pa3.CreatedAt.AddHours(pa3.LoaiPhanAnhId.HanXuLyGio));
        listPhanAnhs.Add(pa3);


        // --- CASE 4: DangXuLy (Processing) - Hong Phat (HaTangKyThuat) ---
        var pa4 = YeuCauPhanAnh.Create(
            hPhatRelation.CanHoId,
            "Đèn chiếu sáng hành lang Block B bị hỏng",
            "Đèn chiếu sáng trước cửa căn hộ B-1502 bị nhấp nháy liên tục từ tối qua, sáng nay thì tắt hẳn, gây tối tăm lối đi hành lang.",
            LoaiPhanAnh.HaTangKyThuat,
            null,
            true).Value;

        pa4.SetCreated(hPhatRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-1));
        pa4.SetHanPhanHoi(pa4.CreatedAt.AddHours(pa4.LoaiPhanAnhId.HanXuLyGio));
        pa4.TiepNhanVaPhanCong(adminId, DateTimeOffset.Now.AddHours(-12));
        listPhanAnhs.Add(pa4);


        // --- CASE 5: CSKHPhanHoi (BQL đã phản hồi) - Giang Kiet (ThaiDoPhucVu) ---
        var pa5 = YeuCauPhanAnh.Create(
            gKietRelation.CanHoId,
            "Bảo vệ hầm xe có thái độ không đúng mực với cư dân",
            "Tối ngày 07/05, khi tôi quẹt thẻ xe máy vào hầm Block A, máy đọc thẻ bị lỗi không nhận diện. Tôi có nhờ bảo vệ trực hỗ trợ thì người này gắt gỏng, có lời lẽ thiếu tôn trọng cư dân.",
            LoaiPhanAnh.ThaiDoPhucVu,
            null,
            true).Value;

        pa5.SetCreated(gKietRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-2));
        pa5.SetHanPhanHoi(pa5.CreatedAt.AddHours(pa5.LoaiPhanAnhId.HanXuLyGio));
        pa5.TiepNhanVaPhanCong(adminId, DateTimeOffset.Now.AddDays(-1));
        pa5.ThemPhanHoi("Chân thành xin lỗi anh vì trải nghiệm không tốt này. BQL đã làm việc với đơn vị bảo vệ an ninh, yêu cầu trích xuất camera thời điểm trên và tạm đình chỉ công tác bảo vệ trực ca đó để viết bản kiểm điểm. BQL muốn hỏi thêm anh có nhớ rõ tên của nhân viên bảo vệ đó trên thẻ tên không ạ?", true);
        listPhanAnhs.Add(pa5);


        // --- CASE 6: CuDanPhanHoi (Cư dân đã phản hồi) - Hong Phat (Khac) ---
        var pa6 = YeuCauPhanAnh.Create(
            hPhatRelation.CanHoId,
            "Đóng góp ý kiến về việc bổ sung thùng phân loại rác tái chế",
            "Tôi thấy chung cư mình hiện tại chưa có thùng phân loại rác tái chế (chai nhựa, giấy, kim loại) riêng biệt ở khu vực tập trung rác. Đề xuất BQL xem xét bổ sung để nâng cao ý thức bảo vệ môi trường.",
            LoaiPhanAnh.Khac,
            null,
            true).Value;

        pa6.SetCreated(hPhatRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-3));
        pa6.SetHanPhanHoi(pa6.CreatedAt.AddHours(pa6.LoaiPhanAnhId.HanXuLyGio));
        pa6.TiepNhanVaPhanCong(adminId, DateTimeOffset.Now.AddDays(-2));
        pa6.ThemPhanHoi("Chào anh, BQL rất hoan nghênh ý kiến đóng góp thiết thực của anh. BQL đang lên kế hoạch triển khai phân loại rác tại nguồn. Cho hỏi anh thấy nên đặt thùng phân loại rác ở mỗi sảnh tầng hay chỉ ở hầm tập trung chính?", true);
        pa6.ThemPhanHoi("Tôi nghĩ trước mắt nên đặt thử nghiệm ở hầm xe tập trung chính và khu vực công viên nội khu trước để đánh giá hiệu quả, sau đó mới nhân rộng lên các sảnh tầng.", false);
        listPhanAnhs.Add(pa6);


        // --- CASE 7: DaHuy (Cancelled/Rejected by BQL) - Giang Kiet (VeSinhMoitruong) ---
        var pa7 = YeuCauPhanAnh.Create(
            gKietRelation.CanHoId,
            "Yêu cầu BQL cho phép nuôi chó lớn (Golden Retriever) trong căn hộ",
            "Tôi chuẩn bị đón một bé chó Golden nặng khoảng 30kg về nuôi, cam kết giữ vệ sinh và xích mỏ khi ra ngoài. Mong BQL duyệt cho tôi đăng ký nuôi.",
            LoaiPhanAnh.VeSinhMoitruong,
            null,
            true).Value;

        pa7.SetCreated(gKietRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-5));
        pa7.SetHanPhanHoi(pa7.CreatedAt.AddHours(pa7.LoaiPhanAnhId.HanXuLyGio));
        pa7.Cancel(adminId, "Theo nội quy quản lý chung cư mục II khoản 4, chung cư nghiêm cấm nuôi chó mèo, động vật cảnh có trọng lượng trên 10kg để đảm bảo an toàn và vệ sinh chung cho cộng đồng.", DateTimeOffset.Now.AddDays(-4));
        listPhanAnhs.Add(pa7);


        // --- CASE 8: Nhap (Draft) - Hong Phat (HaTangKyThuat) ---
        var pa8 = YeuCauPhanAnh.Create(
            hPhatRelation.CanHoId,
            "Kính lan can ban công bị lỏng ốc vít",
            "Tôi kiểm tra thấy tấm kính cường lực lan can ban công nhà mình bị rung lắc nhẹ khi có gió lớn, dường như bị lỏng ốc vít chân đế.",
            LoaiPhanAnh.HaTangKyThuat,
            null,
            false).Value;

        pa8.SetCreated(hPhatRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-1));
        pa8.SetHanPhanHoi(pa8.CreatedAt.AddHours(pa8.LoaiPhanAnhId.HanXuLyGio));
        listPhanAnhs.Add(pa8);


        // --- CASE 9: DaThuHoi (Withdrawn) - Giang Kiet (Khac) ---
        var pa9 = YeuCauPhanAnh.Create(
            gKietRelation.CanHoId,
            "Phản ánh nhầm tiếng ồn khoan đục từ căn hộ kế bên",
            "Hôm qua tôi có nghe tiếng khoan đục tường ồn ào tưởng nhà A-1205 làm, định phản ánh nhưng hóa ra là nhà A-1206 đã đăng ký thi công với BQL từ trước. Tôi xin rút lại phản ánh.",
            LoaiPhanAnh.Khac,
            null,
            false).Value;

        pa9.SetCreated(gKietRelation.NguoiDungId, DateTimeOffset.Now.AddDays(-4));
        pa9.SetHanPhanHoi(pa9.CreatedAt.AddHours(pa9.LoaiPhanAnhId.HanXuLyGio));
        pa9.Withdraw();
        listPhanAnhs.Add(pa9);


        // 3. Add to context and save
        await context.YeuCauPhanAnhs.AddRangeAsync(listPhanAnhs);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {listPhanAnhs.Count} Complaints (PhanAnh)!");
    }
}
