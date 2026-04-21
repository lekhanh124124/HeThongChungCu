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

            var noiDung = faker.Lorem.Paragraph(1);

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
            request.CapNhatThongTinThiCong(null, null, null, "Đã bổ sung hồ sơ theo yêu cầu của BQL. " + faker.Lorem.Sentence(), null, null, null);
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
}
