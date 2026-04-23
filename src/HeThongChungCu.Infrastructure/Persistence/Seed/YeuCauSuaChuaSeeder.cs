using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class YeuCauSuaChuaSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count = 30)
    {
        if (await context.YeuCauSuaChuas.AnyAsync()) return;

        logger.LogInformation("Seeding {Count} Repair Requests (YeuCauSuaChua)...", count);

        var faker = new Faker("vi");

        // 1. Lấy dữ liệu nền tảng
        var technicians = await context.NhanViens
            .Where(n => n.LoaiNhanVienId == LoaiNhanVien.KyThuat)
            .ToListAsync();
        var staffManager = await context.NhanViens
            .Where(n => n.LoaiNhanVienId == LoaiNhanVien.QuanLy)
            .FirstOrDefaultAsync();

        var partnerContracts = await context.HopDongDoiTacs
            .Include(h => h.DoiTac)
            .ToListAsync();

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
            logger.LogWarning("No Apartments with residents found. Skipping YeuCauSuaChua seeding.");
            return;
        }

        var requests = new List<YeuCauSuaChua>();

        for (int i = 0; i < count; i++)
        {
            var aptId = faker.PickRandom(validApartmentIds);
            var phamVi = faker.PickRandom(PhamViSuaChua.GetAll().ToList());
            var loaiSuCo = faker.PickRandom(LoaiSuCoKyThuat.GetAll().ToList());

            var noiDung = GenerateIssueDescription(loaiSuCo, faker);
            var moTaViTri = faker.PickRandom(new[] { "Phòng khách", "Phòng ngủ chính", "Nhà vệ sinh", "Ban công", "Khu vực bếp", "Trần nhà" });

            // Khởi tạo request
            var request = YeuCauSuaChua.Create(aptId, phamVi, loaiSuCo, noiDung, moTaViTri);

            var requesterId = apartmentRequesters[aptId];
            var createdDate = DateTimeOffset.Now.AddDays(-faker.Random.Int(10, 30));
            request.SetCreated(requesterId, createdDate);

            // Quyết định trạng thái mục tiêu
            SeedTargetState[] targetOptions =
            [
                SeedTargetState.Pending,
                SeedTargetState.Rejected,
                SeedTargetState.Approved,
                SeedTargetState.DaDieuPhoi,
                SeedTargetState.DaDuyetBaoGia,
                SeedTargetState.DaHenLich,
                SeedTargetState.Completed,
                SeedTargetState.Cancelled
            ];
            float[] targetWeights = [0.1f, 0.05f, 0.1f, 0.15f, 0.1f, 0.15f, 0.25f, 0.1f];
            var targetStatus = faker.Random.WeightedRandom(targetOptions, targetWeights);

            try
            {
                // Điều hướng workflow
                ApplyWorkflowState(request, targetStatus, technicians, staffManager, partnerContracts, faker, context);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to apply workflow state {targetStatus} for request {i}: {ex.Message}");
            }

            requests.Add(request);
        }

        await context.YeuCauSuaChuas.AddRangeAsync(requests);

        // Clean events before saving
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} Repair Requests.", requests.Count);
    }

    private static string GenerateIssueDescription(LoaiSuCoKyThuat loai, Faker faker)
    {
        return loai.Value switch
        {
            1 => faker.PickRandom(new[] { "Ổ cắm bị chập điện, có mùi khét.", "Mất điện cục bộ ở phòng bếp.", "Nhảy CB liên tục khi bật máy nước nóng.", "Bóng đèn led phòng khách bị nhấp nháy." }),
            2 => faker.PickRandom(new[] { "Vòi sen bị rò rỉ nước.", "Bồn cầu bị nghẹt, nước thoát chậm.", "Đường ống dưới lavabo bị thấm nước ra sàn.", "Áp lực nước yếu, không đủ tắm." }),
            3 => faker.PickRandom(new[] { "Khóa cửa vân tay không nhận diện được.", "Cửa ban công bị kẹt, khó đóng mở.", "Tay nắm cửa phòng ngủ bị lỏng.", "Cửa chính bị xệ, cạ vào sàn nhà." }),
            4 => faker.PickRandom(new[] { "Máy lạnh không lạnh, chỉ ra gió.", "Cục nóng máy lạnh kêu quá to.", "Máy lạnh bị chảy nước trong nhà.", "Điều khiển máy lạnh không hoạt động." }),
            5 => faker.PickRandom(new[] { "Thang máy số 2 rung lắc khi di chuyển.", "Nút bấm tầng 5 trong thang máy bị liệt.", "Thang máy báo lỗi quá tải dù không có người.", "Cửa thang máy đóng mở chậm bất thường." }),
            6 => faker.PickRandom(new[] { "Đèn hành lang bị cháy.", "Hệ thống chiếu sáng khu vực sảnh bị tối.", "Đèn trang trí hồ bơi không sáng.", "Cảm biến ánh sáng sân vườn không hoạt động." }),
            _ => "Cần kiểm tra và sửa chữa thiết bị hư hỏng."
        };
    }

    private enum SeedTargetState
    {
        Pending,
        Rejected,
        Approved,
        DaDieuPhoi,
        DaDuyetBaoGia,
        DaHenLich,
        Completed,
        Cancelled
    }

    private static void ApplyWorkflowState(
        YeuCauSuaChua request,
        SeedTargetState target,
        List<NhanVien> technicians,
        NhanVien? manager,
        List<HopDongDoiTac> contracts,
        Faker faker,
        AppDbContext context)
    {
        var createdDate = request.CreatedAt;
        var handlerId = manager?.Id ?? 1;

        if (target == SeedTargetState.Pending) return;

        // Xử lý từ chối trực tiếp ở bước Pending
        if (target == SeedTargetState.Rejected)
        {
            var rejectDate = createdDate.AddHours(faker.Random.Int(1, 12));
            request.Reject(handlerId, faker.PickRandom(new[] { "Thông tin không hợp lệ.", "Trùng lặp yêu cầu.", "Không thuộc phạm vi hỗ trợ." }), rejectDate);
            return;
        }

        // Determine if it's cancelled and at what stage to cancel it simulating realistic flow.
        int cancelAfterStep = 0;
        if (target == SeedTargetState.Cancelled)
        {
            // Cancelled could happen after Approve(1), DieuPhoi(2), BaoGia(3), or HenLich(4)
            cancelAfterStep = faker.Random.Int(1, 4);
        }

        // Step 1: Tiếp nhận
        bool needsTiepNhan = target >= SeedTargetState.Approved || target == SeedTargetState.Cancelled;
        if (needsTiepNhan)
        {
            var tiepNhanDate = createdDate.AddHours(faker.Random.Int(1, 24));
            request.Approve(handlerId, tiepNhanDate);

            if (target == SeedTargetState.Cancelled && cancelAfterStep == 1)
            {
                CancelRequest(request, handlerId, tiepNhanDate, faker);
                return;
            }
        }

        // Step 2: Điều phối
        bool needsDieuPhoi = target >= SeedTargetState.DaDieuPhoi || (target == SeedTargetState.Cancelled && cancelAfterStep >= 2);
        if (needsDieuPhoi)
        {
            var dieuPhoiDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(2, 12));
            bool usePartner = faker.Random.Bool() && contracts.Any();
            if (usePartner)
            {
                var contract = faker.PickRandom(contracts);
                request.AssignPartner(contract.Id);
                request.AddNhanSuPartner(faker.Name.FullName(), "079" + faker.Random.Number(1000000, 9999999), faker.Phone.PhoneNumber(), "Thợ chính", "Nhân sự từ đối tác " + contract.DoiTac.TenDoiTac);
            }
            else if (technicians.Any())
            {
                var tech = faker.PickRandom(technicians);
                request.AssignInternalStaff([tech.Id]);
            }
            request.SetModified(handlerId, dieuPhoiDate);

            if (target == SeedTargetState.Cancelled && cancelAfterStep == 2)
            {
                CancelRequest(request, handlerId, dieuPhoiDate, faker);
                return;
            }
        }

        // Step 3: Báo giá
        bool needsBaoGia = target >= SeedTargetState.DaDuyetBaoGia || (target == SeedTargetState.Cancelled && cancelAfterStep >= 3);
        if (needsBaoGia)
        {
            var baoGiaDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(4, 24));
            bool isFree = request.HopDongDoiTacId != null || faker.Random.Bool(0.3f);
            decimal cost = isFree ? 0 : faker.Finance.Amount(50000, 500000, 0);
            string ghiChu = isFree
                ? "Hạng mục nằm trong gói bảo trì."
                : $"Cư dân đồng ý qua điện thoại ngày {baoGiaDate:dd/MM/yyyy}. Chi phí thay thế linh kiện chính hãng.";

            request.NhapBaoGia(cost, isFree, ghiChu);
            request.SetModified(handlerId, baoGiaDate);

            if (target == SeedTargetState.Cancelled && cancelAfterStep == 3)
            {
                CancelRequest(request, handlerId, baoGiaDate, faker);
                return;
            }
        }

        // Step 4: Hẹn lịch
        bool needsHenLich = target >= SeedTargetState.DaHenLich || (target == SeedTargetState.Cancelled && cancelAfterStep >= 4);
        if (needsHenLich)
        {
            var scheduleDate = (request.ModifiedAt ?? request.CreatedAt).AddDays(faker.Random.Int(1, 3));
            request.HenLich(scheduleDate, scheduleDate.AddHours(2));
            request.SetModified(handlerId, scheduleDate);

            if (target == SeedTargetState.Cancelled && cancelAfterStep >= 4)
            {
                CancelRequest(request, handlerId, scheduleDate, faker);
                return;
            }
        }

        // Step 5: Hoan tat
        if (target == SeedTargetState.Completed)
        {
            var hoanTatDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(1, 4));
            request.HoanTatXuLy(handlerId, "Đã xử lý dứt điểm sự cố, khách hàng hài lòng.", request.ChiPhiDuKien, hoanTatDate);
            request.SetModified(handlerId, hoanTatDate);
        }
    }

    private static void CancelRequest(YeuCauSuaChua request, int handlerId, DateTimeOffset baseDate, Faker faker)
    {
        request.Cancel(handlerId, faker.PickRandom(new[] { "Khách hàng đổi ý, tự sửa chữa.", "Không liên lạc được với khách hàng.", "Chi phí quá cao, khách hàng không đồng ý." }), baseDate.AddHours(faker.Random.Int(1, 48)));
        request.SetModified(handlerId, request.NgayXuLy!.Value);
    }
}
