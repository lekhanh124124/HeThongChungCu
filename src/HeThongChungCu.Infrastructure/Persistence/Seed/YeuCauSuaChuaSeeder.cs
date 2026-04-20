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

            // Quyết định trạng thái mục tiêu (mix TrangThaiYeuCau và TrangThaiSuaChua)
            object[] targetOptions =
            [
                TrangThaiYeuCau.Pending,
                TrangThaiYeuCau.Approved,
                TrangThaiSuaChua.DaDieuPhoi,
                TrangThaiSuaChua.DaHenLich,
                TrangThaiYeuCau.Completed,
                TrangThaiYeuCau.Cancelled
            ];
            float[] targetWeights = [0.1f, 0.15f, 0.15f, 0.2f, 0.3f, 0.1f];
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

    private static void ApplyWorkflowState(
        YeuCauSuaChua request,
        object target,  // can be TrangThaiYeuCau or TrangThaiSuaChua
        List<NhanVien> technicians,
        NhanVien? manager,
        List<HopDongDoiTac> contracts,
        Faker faker,
        AppDbContext context)
    {
        var createdDate = request.CreatedAt;
        var handlerId = manager?.Id ?? 1;

        bool isCancelled = target == TrangThaiYeuCau.Cancelled;

        // Step 1: Tiếp nhận — Approved + chưa điều phối thì dừng ở đây
        bool needsTiepNhan = target is TrangThaiSuaChua
            || target == TrangThaiYeuCau.Completed
            || target == TrangThaiYeuCau.Cancelled;

        if (needsTiepNhan)
        {
            var tiepNhanDate = createdDate.AddHours(faker.Random.Int(1, 24));
            request.TiepNhan(handlerId, tiepNhanDate);
        }

        // Step 2: Điều phối — xảy ra khi target là DaDieuPhoi trở lên
        bool needsDieuPhoi = (target is TrangThaiSuaChua sc && sc.Value >= TrangThaiSuaChua.DaDieuPhoi.Value)
            || target == TrangThaiYeuCau.Completed
            || target == TrangThaiYeuCau.Cancelled;

        if (needsDieuPhoi)
        {
            var dieuPhoiDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(2, 12));
            bool usePartner = faker.Random.Bool() && contracts.Any();
            if (usePartner)
            {
                var contract = faker.PickRandom(contracts);
                request.AssignPartner(contract.Id);
                request.AddNhanSuPartner(faker.Name.FullName(), "079" + faker.Random.Number(1000000, 9999999), faker.Phone.PhoneNumber(), "Ợ chính", "Nhân sự từ đối tác " + contract.DoiTac.TenDoiTac);
            }
            else if (technicians.Any())
            {
                var tech = faker.PickRandom(technicians);
                request.AssignInternalStaff([tech.Id]);
            }
            request.SetModified(handlerId, dieuPhoiDate);
        }

        // Step 3: Báo giá
        bool needsBaoGia = (target is TrangThaiSuaChua sc2 && sc2.Value >= TrangThaiSuaChua.DaDuyetBaoGia.Value)
            || target == TrangThaiYeuCau.Completed;

        if (needsBaoGia && !isCancelled)
        {
            var baoGiaDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(4, 24));
            bool isFree = request.HopDongDoiTacId != null || faker.Random.Bool(0.3f);
            decimal cost = isFree ? 0 : faker.Finance.Amount(50000, 500000, 0);
            string ghiChu = isFree
                ? "Hạng mục nằm trong gói bảo trì."
                : $"Cư dân đồng ý qua điện thoại ngày {baoGiaDate:dd/MM/yyyy}. Chi phí thay thế linh kiện chính hãng.";

            request.NhapBaoGia(cost, isFree, ghiChu);
            request.SetModified(handlerId, baoGiaDate);
        }

        // Step 4: Hẹn lịch
        bool needsHenLich = (target is TrangThaiSuaChua sc3 && sc3.Value >= TrangThaiSuaChua.DaHenLich.Value)
            || target == TrangThaiYeuCau.Completed;

        if (needsHenLich && !isCancelled)
        {
            var scheduleDate = (request.ModifiedAt ?? request.CreatedAt).AddDays(faker.Random.Int(1, 3));
            request.HenLich(scheduleDate, scheduleDate.AddHours(2));
            request.SetModified(handlerId, scheduleDate);
        }

        // Step 5: Hoàn tất (Completed)
        if (target == TrangThaiYeuCau.Completed)
        {
            var hoanTatDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(1, 4));
            request.HoanTatXuLy("Đã xử lý dứt điểm sự cố, khách hàng hài lòng.", request.ChiPhiDuKien, hoanTatDate);
            request.SetModified(handlerId, hoanTatDate);
        }

        // Step 6: Hủy (Cancelled)
        if (target == TrangThaiYeuCau.Cancelled)
        {
            request.Huy(faker.PickRandom(new[] { "Khách hàng đổi ý, tự sửa chữa.", "Không liên lạc được với khách hàng.", "Hạng mục không nằm trong phạm vi hỗ trợ." }));
            request.SetModified(handlerId, createdDate.AddHours(faker.Random.Int(1, 48)));
        }
    }
}
