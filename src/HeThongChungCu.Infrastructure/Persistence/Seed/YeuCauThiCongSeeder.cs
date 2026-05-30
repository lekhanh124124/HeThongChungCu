using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class YeuCauThiCongSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count = 20)
    {
        if (await context.YeuCauThiCongs.AnyAsync()) return;

        logger.LogInformation("Seeding {Count} Construction Requests (YeuCauThiCong) with comprehensive scenarios...", count);

        var faker = new Faker("vi");

        // 1. Get foundation data
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        // Get residents grouped by apartment to find requesters
        var residentsByApartment = await context.QuanHeCuTrus
            .Where(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .Join(context.TaiKhoan,
                qh => qh.NguoiDungId,
                tk => tk.NguoiDungId,
                (qh, tk) => new { qh.CanHoId, tk.Id, qh.LoaiQuanHeCuTruId })
            .ToListAsync();

        var apartmentRequesters = residentsByApartment
            .GroupBy(r => r.CanHoId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo ? 0 : 1).First().Id
            );

        var validApartmentIds = apartmentRequesters.Keys.ToList();

        if (!validApartmentIds.Any())
        {
            logger.LogWarning("No Apartments with residents found. Skipping YeuCauThiCong seeding.");
            return;
        }

        var requests = new List<YeuCauThiCong>();

        for (int i = 0; i < count; i++)
        {
            var aptId = faker.PickRandom(validApartmentIds);
            var hangMucItems = new[]
            {
                "Cải tạo nội thất phòng khách và bếp",
                "Lát lại sàn gỗ toàn bộ căn hộ",
                "Sơn sửa và chống thấm ban công",
                "Lắp đặt hệ thống điều hòa Multi",
                "Thi công trần thạch cao và đèn led",
                "Cải tạo nhà vệ sinh, thay mới thiết bị",
                "Lắp đặt rèm cửa và lưới an toàn",
                "Sửa chữa hệ thống điện nước âm tường",
                "Lắp đặt hệ thống Smart Home",
                "Cải tạo phòng ngủ thành phòng làm việc"
            };

            var hangMuc = faker.PickRandom(hangMucItems);
            var batDau = DateTimeOffset.Now.AddDays(faker.Random.Int(-30, 10));
            var ketThuc = batDau.AddDays(faker.Random.Int(5, 45));
            
            var tenDonVi = faker.Company.CompanyName();
            var nguoiDaiDien = faker.Name.FullName();
            var sdt = "09" + faker.Random.Number(10000000, 99999999);

            var noiDung = GenerateConstructionDescription(hangMuc, faker);

            // Create request
            var request = YeuCauThiCong.Create(
                aptId, 
                hangMuc, 
                batDau, 
                ketThuc, 
                noiDung, 
                tenDonVi, 
                nguoiDaiDien, 
                sdt,
                trangThaiBanDau: TrangThaiYeuCau.Saved);

            var requesterId = apartmentRequesters[aptId];
            var createdDate = batDau.AddDays(-faker.Random.Int(5, 15));
            request.SetCreated(requesterId, createdDate);

            // Comprehensive Target State distribution
            SeedTargetState[] targetOptions =
            [
                SeedTargetState.Saved,
                SeedTargetState.Withdrawn,
                SeedTargetState.Returned,
                SeedTargetState.Resubmitted,
                SeedTargetState.Pending,
                SeedTargetState.Rejected,
                SeedTargetState.ChoThuCoc,
                SeedTargetState.DaCapPhep,
                SeedTargetState.DaHoanTat,
                SeedTargetState.Completed,
                SeedTargetState.Cancelled
            ];
            float[] targetWeights = [0.05f, 0.05f, 0.05f, 0.10f, 0.10f, 0.05f, 0.10f, 0.15f, 0.10f, 0.20f, 0.05f];
            var targetStatus = faker.Random.WeightedRandom(targetOptions, targetWeights);

            try
            {
                ApplyWorkflowState(request, targetStatus, adminId, faker);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to apply workflow state {targetStatus} for request {i}: {ex.Message}");
            }

            requests.Add(request);
        }

        await context.YeuCauThiCongs.AddRangeAsync(requests);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} Construction Requests with diverse scenarios.", requests.Count);
    }

    private enum SeedTargetState
    {
        Saved,
        Withdrawn,
        Returned,
        Resubmitted,
        Pending,
        Rejected,
        ChoThuCoc,
        DaCapPhep,
        DaHoanTat,
        Completed,
        Cancelled
    }

    private static void ApplyWorkflowState(YeuCauThiCong request, SeedTargetState target, int adminId, Faker faker)
    {
        var baseDate = request.CreatedAt;

        // Files - Always add some files
        var fileCount = faker.Random.Int(1, 4);
        for (int f = 0; f < fileCount; f++)
        {
            var fileName = faker.PickRandom(new[] { "ban-ve-mat-bang.pdf", "hop-dong-thi-cong.docx", "cccd-nha-thau.jpg", "phuong-an-pccc.pdf", "anh-trang-thai-truoc-thi-cong.png" });
            var tep = new TepYeuCauThiCong(
                faker.System.FileName(fileName), 
                faker.System.FilePath(), 
                (long)faker.Random.Int(100000, 10000000), 
                "application/octet-stream");
            request.AddTep(tep);
        }

        if (target == SeedTargetState.Saved) return;

        // Withdrawal from Saved
        if (target == SeedTargetState.Withdrawn && faker.Random.Bool(0.5f))
        {
            request.Withdraw();
            return;
        }

        // Transition to Pending (Submit)
        var submitDate = baseDate.AddMinutes(faker.Random.Int(10, 120));
        request.Submit();

        if (target == SeedTargetState.Pending) return;

        // Rejection Logic
        if (target == SeedTargetState.Rejected)
        {
            var rejectDate = submitDate.AddHours(faker.Random.Int(2, 48));
            var reasons = new[] { 
                "Hạng mục thi công ảnh hưởng đến kết cấu chịu lực của tòa nhà.",
                "Đơn vị thi công nằm trong danh sách đen do vi phạm quy định PCCC trước đó.",
                "Thời gian thi công đăng ký trùng với thời gian bảo trì hệ thống toàn tòa nhà.",
                "Thiếu bản vẽ kỹ thuật chi tiết hệ thống điện nước âm tường."
            };
            request.Reject(adminId, faker.PickRandom(reasons), rejectDate);
            return;
        }

        // Return Logic
        if (target == SeedTargetState.Returned || target == SeedTargetState.Resubmitted || target == SeedTargetState.Withdrawn)
        {
            var returnDate = submitDate.AddHours(faker.Random.Int(1, 24));
            var returnReasons = new[] {
                "Vui lòng bổ sung ảnh chụp CCCD của người đại diện đơn vị thi công.",
                "Bản vẽ mặt bằng cần thể hiện rõ vị trí lắp đặt cục nóng điều hòa.",
                "Cần đính kèm hợp đồng bảo hiểm rủi ro trong thi công.",
                "Thông tin thời gian dự kiến chưa hợp lý, cần giãn cách ngày kết thúc."
            };
            request.Return(adminId, faker.PickRandom(returnReasons), returnDate);

            if (target == SeedTargetState.Returned) return;
            
            if (target == SeedTargetState.Withdrawn)
            {
                request.Withdraw();
                return;
            }

            // Resubmit Loop
            var updateDate = returnDate.AddDays(faker.Random.Int(1, 3));
            var boSungComments = new[]
            {
                "Tôi gửi kèm bản chụp CCCD mới của tổ trưởng thi công và giấy chứng nhận kiểm định an toàn của máy hàn điện.",
                "Đã đính kèm bản vẽ điều chỉnh vị trí đặt cục nóng điều hòa theo đúng thiết kế của Block.",
                "Đã bổ sung phụ lục hợp đồng cam kết bảo hiểm trách nhiệm công cộng đối với bên thứ ba.",
                "Đã điều chỉnh lại thời gian dự kiến kết thúc lùi lại 3 ngày để phù hợp tiến độ thực tế."
            };
            request.CapNhatThongTinThiCong(null, null, null, "Đã bổ sung hồ sơ theo yêu cầu của BQL. " + faker.PickRandom(boSungComments), null, null, null);
            request.Submit();
            // Now state is Pending again
            if (target == SeedTargetState.Resubmitted && faker.Random.Bool(0.3f)) return; // Some stay pending
        }

        // Processing / Approved
        var processedDate = (request.NgayXuLy ?? baseDate).AddHours(faker.Random.Int(12, 72));

        // Add Personnel (Contractors) 
        var staffCount = faker.Random.Int(2, 8);
        for (int p = 0; p < staffCount; p++)
        {
            var gender = faker.PickRandom(new[] { 1, 2 });
            var birthYear = DateTime.Now.Year - faker.Random.Int(18, 60);
            var cccd = (gender == 1 ? "0" : "1") + faker.Random.Number(10, 99) + (birthYear % 100).ToString("D2") + faker.Random.Number(1000000, 9999999);
            
            request.AddNhanSu(
                faker.Name.FullName(), 
                cccd, 
                "0" + faker.PickRandom(new[]{"3","5","7","8","9"}) + faker.Random.Number(10000000, 99999999), 
                p == 0 ? "Chỉ huy trưởng" : (p == 1 ? "Kỹ thuật viên" : "Công nhân"),
                "Nhà thầu " + request.TenDonViThiCong);
        }

        // Approved (ChoThuCoc)
        var approveDate = processedDate.AddHours(faker.Random.Int(2, 24));
        var depositAmount = faker.PickRandom(new decimal[] { 5000000, 10000000, 15000000, 20000000, 30000000, 50000000 });
        request.SetTienDatCoc(depositAmount);
        request.Approve(adminId, approveDate);

        if (target == SeedTargetState.ChoThuCoc) return;

        // DaCapPhep (Confirm Deposit)
        if (target >= SeedTargetState.DaCapPhep || target == SeedTargetState.Cancelled)
        {
            var confirmDate = approveDate.AddDays(faker.Random.Int(1, 5));
            var ways = new[] { "Tiền mặt tại quầy lễ tân", "Chuyển khoản Vietcombank", "Cà thẻ POS", "Chuyển khoản App MB Bank" };
            request.XacNhanThuCoc($"Xác nhận đã nhận tiền đặt cọc qua {faker.PickRandom(ways)}.");

            if (target == SeedTargetState.Cancelled)
            {
                var cancelDate = confirmDate.AddDays(faker.Random.Int(1, 7));
                request.Cancel(adminId, "Khách hàng yêu cầu hủy đơn và hoàn cọc do không tìm được đội thi công phù hợp.", cancelDate);
                return;
            }
        }

        if (target == SeedTargetState.DaCapPhep) return;

        // DaHoanTat (Construction Finished)
        var finishedDate = request.DuKienKetThuc.AddDays(faker.Random.Int(-5, 10));
        request.HoanTatThiCong();

        if (target == SeedTargetState.DaHoanTat) return;

        // Refund (HoanCoc)
        var refundDate = finishedDate.AddDays(faker.Random.Int(1, 10));
        bool hasDeduction = faker.Random.Bool(0.25f);
        
        decimal khauTru = 0;
        string? lyDo = "Nghiệm thu đạt yêu cầu, hoàn trả 100% tiền cọc.";

        if (hasDeduction)
        {
            var deductionScenarios = new[] {
                new { Reason = "Vi phạm thời gian thi công (quá giờ quy định 3 lần).", Amount = 500000m },
                new { Reason = "Làm hư hại sơn tường khu vực hành lang căn hộ.", Amount = 1200000m },
                new { Reason = "Gây bẩn khu vực thang máy vận chuyển vật liệu.", Amount = 300000m },
                new { Reason = "Làm nứt gạch lát sảnh chung trong quá trình vận chuyển thạch cao.", Amount = 2500000m },
                new { Reason = "Thi công gây tiếng ồn vượt mức cho phép trong giờ nghỉ trưa.", Amount = 1000000m }
            };
            var scenario = faker.PickRandom(deductionScenarios);
            khauTru = Math.Min(scenario.Amount, request.TienDatCoc ?? 0);
            lyDo = scenario.Reason;
        }
        
        request.HoanCoc(khauTru, lyDo);

        // Completed
        if (target == SeedTargetState.Completed)
        {
            var completeDate = refundDate.AddHours(faker.Random.Int(4, 48));
            request.Complete(adminId, completeDate);
        }
    }

    private static string GenerateConstructionDescription(string hangMuc, Faker faker)
    {
        return hangMuc switch
        {
            "Cải tạo nội thất phòng khách và bếp" => faker.PickRandom(new[]
            {
                "Thi công làm mới hệ thống tủ bếp kịch trần bằng gỗ MDF chống ẩm An Cường, lắp đặt mặt đá bếp thạch anh chống ố và ốp kính cường lực. Cải tạo hệ tủ tivi, vách ngăn phòng khách dạng lam gỗ hiện đại.",
                "Cải tạo lại không gian phòng khách kết hợp gian bếp mở: Đóng mới tủ bếp chữ L, quầy bar mini tiện lợi, lắp đặt tủ giày kịch trần sảnh vào và vách ốp trang trí tivi phòng khách.",
                "Thay thế tủ bếp cũ bị mối mọt bằng tủ bếp khung cánh nhựa Picomat cánh phủ Acrylic bóng gương cao cấp, lắp đặt phụ kiện tủ bếp thông minh thương hiệu Hafele."
            }),
            "Lát lại sàn gỗ toàn bộ căn hộ" => faker.PickRandom(new[]
            {
                "Tháo dỡ lớp gạch men cũ hiện tại bị rộp bong tróc, tiến hành xử lý cán phẳng lại nền và lát sàn gỗ công nghiệp dày 12mm chịu nước thương hiệu Kronoswiss cho toàn bộ phòng khách và 3 phòng ngủ.",
                "Thi công lót sàn gỗ xương cá cao cấp cho toàn bộ căn hộ (trừ khu vực nhà vệ sinh và ban công). Có sử dụng lớp cao su non 3mm lót sàn cách âm tốt.",
                "Cải tạo lại sàn nhà: Tháo dỡ sàn cũ bị cong vênh do ngập nước trước đây, cán nền phẳng bằng vữa tự san phẳng và tiến hành lát lại sàn gỗ công nghiệp cốt xanh chống ẩm nhập khẩu Malaysia."
            }),
            "Sơn sửa và chống thấm ban công" => faker.PickRandom(new[]
            {
                "Đục bỏ lớp gạch ban công hiện tại bị nứt nẻ thấm ẩm xuống nhà dưới, thi công quét 3 lớp chống thấm ngược dạng màng đàn hồi Sika Lastic, cán nền tạo độ dốc thoát nước tốt và lát gạch chống trơn trượt mới.",
                "Khắc phục tường ban công bị ẩm mốc bong tróc sơn do thời tiết mưa tạt: Cạo sủi lớp sơn cũ, bả matit chống thấm ngoài trời và lăn lại 2 lớp sơn bóng phủ chống bám bụi Dulux Weathershield.",
                "Thi công chống thấm dột khu vực lô gia phơi đồ, nâng cao bậc thềm cửa ngăn nước mưa tràn vào nhà, lắp đặt phễu thoát nước sàn chống mùi hôi chuyên dụng."
            }),
            "Lắp đặt hệ thống điều hòa Multi" => faker.PickRandom(new[]
            {
                "Lắp đặt hệ thống máy lạnh Multi Daikin Inverter gồm 1 dàn nóng công suất 4.0 HP đặt ngoài ban công kết nối với 3 dàn lạnh âm trần nối ống gió sang trọng cho phòng khách và 2 phòng ngủ.",
                "Thi công đi âm đường ống đồng, ống nước thải máy lạnh cho hệ thống điều hòa Multi Panasonic 1 nóng 4 lạnh phục vụ toàn bộ các phòng trong căn hộ.",
                "Lắp đặt trọn gói hệ thống điều hòa Multi Mitsubishi gồm 1 dàn nóng 34000BTU tiết kiệm diện tích ban công và 3 dàn lạnh treo tường dòng cao cấp có bộ lọc khí kháng khuẩn."
            }),
            "Thi công trần thạch cao và đèn led" => faker.PickRandom(new[]
            {
                "Đóng trần thạch cao khung xương chìm Vĩnh Tường chống ẩm cho toàn nhà, đi lại dây điện nguồn âm trần, khoét lỗ lắp đặt hệ thống đèn Led âm trần downlight 9W ánh sáng trung tính và đèn led dây hắt trần trang trí.",
                "Cải tạo lại trần nhà: Hạ trần thạch cao giật cấp trang trí phòng khách và bếp, đi hệ thống đèn rọi ray hiện đại tạo điểm nhấn không gian ấm cúng.",
                "Thi công đóng trần phẳng thạch cao chống ẩm tấm Gyproc cho khu vực nhà vệ sinh và sảnh vào hành lang căn hộ, tích hợp quạt thông gió âm trần và đèn led cảm ứng."
            }),
            "Cải tạo nhà vệ sinh, thay mới thiết bị" => faker.PickRandom(new[]
            {
                "Đục toàn bộ gạch ốp lát tường và sàn nhà vệ sinh master, quét chống thấm polyurethane toàn bộ bề mặt sàn và tường cao 1.8m, ốp lát gạch đá Viglacera 30x60 sang trọng, lắp vách tắm kính cường lực và bộ thiết bị vệ sinh Toto.",
                "Cải tạo nâng cấp nhà tắm chung: Thay mới bồn cầu thông minh, lắp tủ chậu lavabo mặt đá chống nước, thay vòi sen cây tắm massage và lắp đặt bình nóng lạnh gián tiếp Ariston 30L.",
                "Xử lý chống thấm cổ ống thoát sàn nhà vệ sinh bị rò rỉ nước, thi công lát lại gạch sàn chống trượt taicera và lắp phễu thu nước ngăn mùi hôi ngăn côn trùng."
            }),
            "Lắp đặt rèm cửa và lưới an toàn" => faker.PickRandom(new[]
            {
                "Lắp đặt hệ thống rèm vải 2 lớp (1 lớp vải chống nắng cản sáng 100%, 1 lớp voan trắng nhẹ nhàng) cho phòng khách và các phòng ngủ. Thi công lưới an toàn cáp inox bọc nhựa bảo vệ ban công đảm bảo an toàn cho trẻ nhỏ.",
                "Thi công lắp đặt rèm cầu vồng Hàn Quốc hiện đại cho các ô cửa sổ phòng ngủ phụ, rèm sáo gỗ cho phòng làm việc và lưới an toàn ban công gia cố lực kéo cực tốt.",
                "Lắp đặt lưới sợi thủy tinh chống côn trùng, muỗi cho toàn bộ cửa sổ căn hộ kết hợp hệ thống rèm cuốn tự động tích hợp Smart Home."
            }),
            "Sửa chữa hệ thống điện nước âm tường" => faker.PickRandom(new[]
            {
                "Cải tạo đi lại đường dây điện cấp nguồn riêng biệt cho bếp từ công suất lớn và máy rửa bát âm tủ, lắp đặt thêm các ổ cắm điện âm tường Panasonic tại khu vực phòng khách và phòng làm việc.",
                "Dịch chuyển vị trí đường ống cấp và thoát nước sinh hoạt trong phòng bếp để phù hợp với bản vẽ thiết kế tủ bếp mới, thi công đấu nối lại van giảm áp nguồn cấp tổng căn hộ.",
                "Khắc phục sự cố rò rỉ đường ống nước sạch chịu nhiệt PPR đi âm tường khu vực nhà tắm master, thi công thay thế đoạn ống nước bị nứt vỡ và hoàn trả lại mặt bằng tường ốp."
            }),
            "Lắp đặt hệ thống Smart Home" => faker.PickRandom(new[]
            {
                "Thi công thay thế toàn bộ công tắc cơ thông thường bằng hệ thống công tắc cảm ứng thông minh viền vàng Lumi, lắp đặt bộ điều khiển trung tâm HC, hệ thống cảm biến chuyển động cầu thang và rèm tự động.",
                "Nâng cấp căn hộ thông minh Smart Home dòng Tuya: Lắp khóa cửa thông minh nhận diện khuôn mặt vân tay FaceID, hệ thống điều khiển bình nóng lạnh, điều hòa và tivi qua giọng nói tiếng Việt.",
                "Lắp đặt hệ thống an ninh thông minh gồm cảm biến mở cửa, cảm biến phát hiện rò rỉ nước tại bếp và còi báo động kết nối cảnh báo tức thì qua điện thoại của chủ hộ."
            }),
            "Cải tạo phòng ngủ thành phòng làm việc" => faker.PickRandom(new[]
            {
                "Tháo dỡ giường tủ cũ tại phòng ngủ nhỏ, thi công đóng mới hệ bàn làm việc đôi kết hợp giá sách kịch trần rộng rãi, lắp đặt vách ốp tiêu âm cách âm cho phòng họp online tại nhà.",
                "Thiết kế thi công hệ giường gấp thông minh kết hợp bàn làm việc đa năng tối ưu diện tích phòng ngủ phụ thành phòng làm việc ban ngày và phòng ngủ khách ban đêm.",
                "Lắp đặt hệ thống tủ hồ sơ tài liệu nhiều ngăn kéo bằng gỗ công nghiệp Melamine chống trầy xước, bàn làm việc chữ L rộng rãi có chân sắt sơn tĩnh điện vô cùng chắc chắn."
            }),
            _ => "Tiến hành cải tạo, sửa chữa và nâng cấp các hạng mục nội thất, trang thiết bị trong căn hộ theo đúng quy định và tiêu chuẩn kỹ thuật của tòa nhà."
        };
    }
}
