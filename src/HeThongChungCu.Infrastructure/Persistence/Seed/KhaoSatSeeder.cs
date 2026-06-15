using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class KhaoSatSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.KhaoSats.AnyAsync())
        {
            logger.LogInformation("Surveys (KhaoSat) already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding Surveys and Resident Votes...");

        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 1;

        // --- 1. SEED SURVIES (KhaoSat) AND QUESTIONS/OPTIONS ---
        
        // Campaign 1: Board of Representatives Election (BauCuBanQuanTri) - Ongoing
        var ks1 = KhaoSat.Create(
            "Bầu cử Ban Quản trị Chung cư nhiệm kỳ 2026 - 2028",
            "Biểu quyết bầu chọn các thành viên đại diện cư dân tham gia vào Ban Quản trị chung cư nhiệm kỳ mới nhằm giám sát vận hành và quản lý quỹ bảo trì.",
            LoaiKhaoSat.BauCuBanQuanTri,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(-5),
            DateTimeOffset.Now.AddDays(10),
            false).Value;

        var candidates = new List<(string NoiDung, bool IsUngVien, string? TieuSu, int? UngVienId)>
        {
            ("Ông Nguyễn Văn Hùng", true, "Cựu kỹ sư xây dựng, 15 năm kinh nghiệm, hiện là trưởng ban đại diện cư dân block A", 101),
            ("Bà Lê Thị Mai", true, "Thạc sĩ Quản trị Kinh doanh, Trưởng phòng Nhân sự tập đoàn đa quốc gia, cư trú tại căn B-1012", 102),
            ("Ông Trần Hoài Nam", true, "Luật sư thành viên đoàn luật sư TP.HCM, có kinh nghiệm tư vấn pháp lý chung cư", 103),
            ("Bà Phạm Thị Tuyết", true, "Kế toán trưởng công ty kiểm toán, có kinh nghiệm quản lý tài chính quỹ công cộng", 104)
        };
        ks1.ThemCauHoi("Vui lòng chọn tối đa 3 ứng cử viên xuất sắc nhất ứng cử vào Ban Quản trị chung cư:", true, true, candidates);
        ks1.PublicCampaign(); // Move draft -> DangDienRa


        // Campaign 2: Management Fee Adjustment (BieuQuyetNghiQuyet - Weighted Area Voting)
        var ks2 = KhaoSat.Create(
            "Biểu quyết thông qua Nghị quyết điều chỉnh đơn giá phí quản lý vận hành năm 2026",
            "Ban Quản lý đề xuất điều chỉnh phí quản lý vận hành Block A từ 10.000đ/m² lên 11.000đ/m² nhằm tăng cường tần suất làm sạch, bảo trì thang máy Otis, nâng cấp camera giám sát và chăm sóc cảnh quan xanh.",
            LoaiKhaoSat.BieuQuyetNghiQuyet,
            CoCheTinhDiemBauCu.TheoDienTichSoHuu,
            DateTimeOffset.Now.AddDays(-3),
            DateTimeOffset.Now.AddDays(7),
            false).Value;

        ks2.ThemCauHoi("Ý kiến của quý cư dân về việc thông qua Nghị quyết điều chỉnh đơn giá phí quản lý vận hành:", true, false, new List<string> {
            "Đồng ý (Thông qua Nghị quyết)",
            "Không đồng ý (Giữ nguyên đơn giá cũ)",
            "Ý kiến khác (Vui lòng ghi rõ ở phần ý kiến tự do)"
        });
        ks2.PublicCampaign();


        // Campaign 3: Utilities Service Quality Survey (LayYKienCuDan) - Ended
        var ks3 = KhaoSat.Create(
            "Khảo sát ý kiến cư dân về chất lượng dịch vụ dọn dẹp bTaskee và Gym California",
            "Khảo sát định kỳ hàng năm nhằm đánh giá chất lượng dịch vụ của đối tác giúp việc bTaskee và phòng Gym California Fitness tại tầng tiện ích, làm cơ sở gia hạn hợp đồng.",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(-20),
            DateTimeOffset.Now.AddDays(-5),
            true).Value;

        ks3.ThemCauHoi("Anh/Chị đánh giá thế nào về chất lượng dọn dẹp của nhân viên bTaskee?", true, false, new List<string> {
            "Rất sạch sẽ & chu đáo",
            "Đạt yêu cầu",
            "Chưa đạt yêu cầu, dọn dẹp hời hợt",
            "Không sử dụng dịch vụ này"
        });
        ks3.ThemCauHoi("Trang thiết bị tại phòng Gym California có hoạt động tốt và đầy đủ không?", true, false, new List<string> {
            "Rất hiện đại & hoạt động tốt",
            "Hoạt động bình thường",
            "Nhiều máy bị hỏng hóc chưa sửa kịp thời",
            "Không sử dụng phòng Gym"
        });
        ks3.PublicCampaign();
        ks3.EndCampaign(); // Move DangDienRa -> DaKetThuc


        // Campaign 4: EV Charger Survey (LayYKienCuDan) - Draft
        var ks4 = KhaoSat.Create(
            "Khảo sát nhu cầu lắp đặt trạm sạc xe máy điện và ô tô điện tại hầm xe",
            "Khảo sát mức độ quan tâm và nhu cầu sử dụng trạm sạc xe điện của cư dân phục vụ kế hoạch bố trí vị trí sạc an toàn phòng chống cháy nổ.",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(5),
            DateTimeOffset.Now.AddDays(15),
            false).Value;

        ks4.ThemCauHoi("Anh/Chị hiện đang sử dụng hoặc có kế hoạch mua xe điện trong 6 tháng tới không?", true, false, new List<string> {
            "Đang sử dụng xe máy điện",
            "Đang sử dụng ô tô điện",
            "Có kế hoạch mua xe điện",
            "Chỉ sử dụng phương tiện chạy xăng/dầu truyền thống"
        });
        // Stays in MoiTao (Draft)


        // Campaign 5: Pet Regulation Survey (LayYKienCuDan) - Suspended
        var ks5 = KhaoSat.Create(
            "Khảo sát ý kiến về việc ban hành quy chế nuôi thú cưng trong chung cư",
            "Do xảy ra một số trường hợp chó thả rông không rọ mõm phóng uế tại công viên, BQL tổ chức khảo sát lấy ý kiến cư dân về việc siết chặt quy định quản lý vật nuôi.",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(-10),
            DateTimeOffset.Now.AddDays(20),
            false).Value;

        ks5.ThemCauHoi("Anh/Chị ủng hộ phương án quản lý thú cưng nào dưới đây?", true, false, new List<string> {
            "Cấm hoàn toàn việc nuôi chó, mèo trong căn hộ",
            "Cho phép nuôi nhưng phạt cực nặng nếu thả rông hoặc làm mất vệ sinh",
            "Chỉ cho phép nuôi chó nhỏ dưới 5kg và phải đăng ký với BQL"
        });
        ks5.PublicCampaign();
        
        // Force status to TamDung (Suspended) via reflection helper since there's no public transition method
        SetPrivateProperty(ks5, nameof(ks5.TrangThaiId), TrangThaiKhaoSat.TamDung);

        // Campaign 6: Security Camera System Upgrade (LayYKienCuDan) - Ongoing
        var ks6 = KhaoSat.Create(
            "Khảo sát nâng cấp hệ thống Camera giám sát tích hợp AI nhận diện khuôn mặt",
            "Ban Quản lý đề xuất trang bị thêm hệ thống camera AI tại các lối ra vào và sảnh thang máy nhằm tăng cường an ninh, nhận diện người lạ và cảnh báo sớm các hành vi bất thường.",
            LoaiKhaoSat.LayYKienCuDan,
            CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
            DateTimeOffset.Now.AddDays(-2),
            DateTimeOffset.Now.AddDays(15),
            false).Value;

        ks6.ThemCauHoi("Anh/Chị có ủng hộ việc lắp đặt camera AI để tăng cường an ninh không?", true, true, new List<string> {
            "Rất ủng hộ",
            "Ủng hộ nhưng lo ngại về quyền riêng tư",
            "Không cần thiết",
            "Phản đối"
        });
        
        ks6.ThemCauHoi("Ý kiến đóng góp khác của Anh/Chị về việc nâng cao an ninh chung cư:", false, true, new List<string> {
            "Tôi có ý kiến đóng góp khác (Ghi rõ ở phần ý kiến tự do)",
            "Tôi không có ý kiến gì thêm"
        });

        ks6.PublicCampaign();

        // Save surveys first to get primary key IDs populated
        var surveys = new List<KhaoSat> { ks1, ks2, ks3, ks4, ks5, ks6 };
        foreach (var s in surveys)
        {
            s.SetCreated(adminId, s.NgayBatDau);
        }
        await context.KhaoSats.AddRangeAsync(surveys);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Surveys and Questions successfully created in Database.");


        // Get all active residents joined with their apartments to retrieve areas and IDs safely
        var activeResidents = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue)
            .Join(context.CanHos,
                r => r.CanHoId,
                c => c.Id,
                (r, c) => new { 
                    NguoiDungId = r.NguoiDungId, 
                    CanHoId = r.CanHoId, 
                    DienTich = c.ThongSo.DienTich 
                })
            .ToListAsync();

        if (!activeResidents.Any())
        {
            logger.LogWarning("No active residents found to seed votes. Skipping vote seeding.");
            return;
        }

        logger.LogInformation($"Found {activeResidents.Count} active resident records to generate votes.");

        var specialEmails = new[] { "giangkiet2k4@gmail.com", "hongphat@gmail.com" };
        var specialUserAccounts = await context.TaiKhoan.IgnoreQueryFilters()
            .Where(t => specialEmails.Contains(t.Email.Value))
            .ToListAsync();
        var specialUserIds = specialUserAccounts
            .Where(a => a.NguoiDungId.HasValue)
            .Select(a => a.NguoiDungId!.Value)
            .ToList();

        // Load questions & options with IDs
        var dbKs1 = await context.KhaoSats.Include(k => k.CauHois).ThenInclude(q => q.LuaChons).FirstAsync(k => k.Id == ks1.Id);
        var dbKs2 = await context.KhaoSats.Include(k => k.CauHois).ThenInclude(q => q.LuaChons).FirstAsync(k => k.Id == ks2.Id);
        var dbKs3 = await context.KhaoSats.Include(k => k.CauHois).ThenInclude(q => q.LuaChons).FirstAsync(k => k.Id == ks3.Id);
        var dbKs6 = await context.KhaoSats.Include(k => k.CauHois).ThenInclude(q => q.LuaChons).FirstAsync(k => k.Id == ks6.Id);

        var q1 = dbKs1.CauHois.First();
        var optList1 = q1.LuaChons.ToList();

        var q2 = dbKs2.CauHois.First();
        var optList2 = q2.LuaChons.ToList();

        var q3_1 = dbKs3.CauHois.First();
        var optList3_1 = q3_1.LuaChons.ToList();

        var q3_2 = dbKs3.CauHois.Skip(1).First();
        var optList3_2 = q3_2.LuaChons.ToList();

        var q6_1 = dbKs6.CauHois.First();
        var optList6_1 = q6_1.LuaChons.ToList();

        var q6_2 = dbKs6.CauHois.Skip(1).First();
        var optList6_2 = q6_2.LuaChons.ToList();

        var votes = new List<BieuQuyetCuDan>();
        var random = new Random();

        foreach (var res in activeResidents)
        {
            var isSpecialUser = specialUserIds.Contains(res.NguoiDungId);
            var isGiangKiet = isSpecialUser && specialUserAccounts.Any(a => a.NguoiDungId == res.NguoiDungId && a.Email.Value == "giangkiet2k4@gmail.com");
            var isHongPhat = isSpecialUser && specialUserAccounts.Any(a => a.NguoiDungId == res.NguoiDungId && a.Email.Value == "hongphat@gmail.com");

            // --- VOTE FOR SURVEY 1 (Board of Representatives Election - Multi-select) ---
            var choices1 = new List<(int, string?)>();
            if (isGiangKiet)
            {
                // Giang Kiet votes for Nguyễn Văn Hùng and Trần Hoài Nam
                choices1.Add((optList1[0].Id, null));
                choices1.Add((optList1[2].Id, null));
            }
            else if (isHongPhat)
            {
                // Hong Phat votes for Lê Thị Mai, Trần Hoài Nam and Phạm Thị Tuyết
                choices1.Add((optList1[1].Id, null));
                choices1.Add((optList1[2].Id, null));
                choices1.Add((optList1[3].Id, null));
            }
            else
            {
                // Randomly choose 1-3 candidates
                int count = random.Next(1, 4);
                var shuffled = optList1.OrderBy(x => random.Next()).Take(count).ToList();
                foreach (var opt in shuffled)
                {
                    choices1.Add((opt.Id, null));
                }
            }

            var vote1 = BieuQuyetCuDan.Create(
                ks1.Id,
                res.CanHoId,
                res.DienTich,
                CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
                choices1,
                true).Value;

            vote1.SetCreated(res.NguoiDungId, ks1.NgayBatDau.AddHours(random.Next(2, 48)));
            votes.Add(vote1);


            // --- VOTE FOR SURVEY 2 (Management Fee - Single-select, weighted by area!) ---
            var choices2 = new List<(int, string?)>();
            if (isGiangKiet || isHongPhat)
            {
                // Special users agree with the increase to upgrade gym/elevators/CCTV
                choices2.Add((optList2[0].Id, null));
            }
            else
            {
                // 60% agree, 30% disagree, 10% other comments
                var roll = random.Next(100);
                if (roll < 60)
                {
                    choices2.Add((optList2[0].Id, null));
                }
                else if (roll < 90)
                {
                    choices2.Add((optList2[1].Id, null));
                }
                else
                {
                    choices2.Add((optList2[2].Id, "Cần làm rõ chi tiết bảng dự toán chi phí trước khi quyết định tăng giá."));
                }
            }

            var vote2 = BieuQuyetCuDan.Create(
                ks2.Id,
                res.CanHoId,
                res.DienTich,
                CoCheTinhDiemBauCu.TheoDienTichSoHuu,
                choices2,
                true).Value;

            vote2.SetCreated(res.NguoiDungId, ks2.NgayBatDau.AddHours(random.Next(1, 36)));
            votes.Add(vote2);


            // --- VOTE FOR SURVEY 3 (Utilities Service Quality - Ended) ---
            var choices3 = new List<(int, string?)>();
            if (isGiangKiet)
            {
                choices3.Add((optList3_1[0].Id, null)); // bTaskee: Rất sạch sẽ & chu đáo
                choices3.Add((optList3_2[1].Id, null)); // Gym: Bình thường
            }
            else if (isHongPhat)
            {
                choices3.Add((optList3_1[1].Id, null)); // bTaskee: Đạt yêu cầu
                choices3.Add((optList3_2[0].Id, null)); // Gym: Rất hiện đại
            }
            else
            {
                choices3.Add((optList3_1[random.Next(optList3_1.Count)].Id, null));
                choices3.Add((optList3_2[random.Next(optList3_2.Count)].Id, null));
            }

            var vote3 = BieuQuyetCuDan.Create(
                ks3.Id,
                res.CanHoId,
                res.DienTich,
                CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
                choices3,
                true).Value;

            vote3.SetCreated(res.NguoiDungId, ks3.NgayBatDau.AddHours(random.Next(1, 100)));
            votes.Add(vote3);

            // --- VOTE FOR SURVEY 6 (Security Camera Upgrade - Ongoing) ---
            var choices6 = new List<(int, string?)>();
            var isOtpVerified6 = true;
            
            if (isGiangKiet)
            {
                choices6.Add((optList6_1[0].Id, null)); // Rất ủng hộ
                choices6.Add((optList6_2[0].Id, "Cần lắp thêm ở các góc khuất hành lang tầng 5.")); 
            }
            else if (isHongPhat)
            {
                choices6.Add((optList6_1[1].Id, "Cần cam kết không sử dụng dữ liệu cho mục đích khác ngoài an ninh.")); // Ủng hộ nhưng lo ngại
                choices6.Add((optList6_2[0].Id, "Tôi muốn biết thêm về chi phí vận hành hàng tháng."));
            }
            else
            {
                var roll = random.Next(100);
                if (roll < 70) 
                {
                    choices6.Add((optList6_1[0].Id, null));
                    if (random.Next(100) < 30) choices6.Add((optList6_2[0].Id, "Tuyệt vời!"));
                }
                else if (roll < 90) choices6.Add((optList6_1[1].Id, null));
                else if (roll < 95) choices6.Add((optList6_1[2].Id, null));
                else
                {
                    choices6.Add((optList6_1[3].Id, null));
                    isOtpVerified6 = false; // Simulate some residents started but didn't verify OTP
                }
            }

            var vote6 = BieuQuyetCuDan.Create(
                ks6.Id,
                res.CanHoId,
                res.DienTich,
                CoCheTinhDiemBauCu.MoiCanHoMotPhieu,
                choices6,
                isOtpVerified6).Value;

            vote6.SetCreated(res.NguoiDungId, ks6.NgayBatDau.AddHours(random.Next(1, 48)));
            votes.Add(vote6);
        }

        // Add votes to database and save
        await context.BieuQuyetCuDans.AddRangeAsync(votes);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation($"Successfully seeded {votes.Count} verified resident votes across 3 active campaigns!");
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var type = obj.GetType();
        var prop = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
        else
        {
            var field = type.GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
