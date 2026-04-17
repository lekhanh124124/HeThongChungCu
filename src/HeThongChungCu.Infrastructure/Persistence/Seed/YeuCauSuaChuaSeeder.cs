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
            var uuTien = faker.PickRandom(MucDoUuTien.GetAll().ToList());

            var noiDung = GenerateIssueDescription(loaiSuCo, faker);
            var moTaViTri = faker.PickRandom(new[] { "Phòng khách", "Phòng ngủ chính", "Nhà vệ sinh", "Ban công", "Khu vực bếp", "Trần nhà" });

            // Khởi tạo request
            var request = YeuCauSuaChua.Create(aptId, phamVi, loaiSuCo, uuTien, noiDung, moTaViTri);

            var requesterId = apartmentRequesters[aptId];
            var createdDate = DateTimeOffset.Now.AddDays(-faker.Random.Int(10, 30));
            request.SetCreated(requesterId, createdDate);

            // Quyết định trạng thái mục tiêu
            var targetStatus = faker.Random.WeightedRandom(
                [
                    TrangThaiSuaChua.MoiTao,
                    TrangThaiSuaChua.DaTiepNhan,
                    TrangThaiSuaChua.DaDieuPhoi,
                    TrangThaiSuaChua.DangXuLy,
                    TrangThaiSuaChua.DaXuLy,
                    TrangThaiSuaChua.DaDong,
                    TrangThaiSuaChua.DaHuy
                ],
                [0.1f, 0.1f, 0.1f, 0.2f, 0.3f, 0.1f, 0.1f]
            );

            try
            {
                // Điều hướng workflow
                ApplyWorkflowState(request, targetStatus, technicians, staffManager, partnerContracts, faker, context);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to apply workflow state {targetStatus.Name} for request {i}: {ex.Message}");
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
        TrangThaiSuaChua target,
        List<NhanVien> technicians,
        NhanVien? manager,
        List<HopDongDoiTac> contracts,
        Faker faker,
        AppDbContext context)
    {
        var createdDate = request.CreatedAt;
        var handlerId = manager?.Id ?? 1;

        // Step 1: Tiếp nhận
        if (target.Value >= TrangThaiSuaChua.DaTiepNhan.Value && target != TrangThaiSuaChua.DaHuy)
        {
            var tiepNhanDate = createdDate.AddHours(faker.Random.Int(1, 24));
            request.TiepNhan(handlerId, tiepNhanDate);
            request.ChotUuTien(handlerId, request.MucDoUuTienDeXuatId, tiepNhanDate.AddHours(1));
        }

        // Step 2: Điều phối
        if (target.Value >= TrangThaiSuaChua.DaDieuPhoi.Value && target != TrangThaiSuaChua.DaHuy)
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
                request.AssignInternalStaff(tech.Id);
            }

            request.XacNhanKiemTra();
            // Simulating internal state update for date progression
            request.SetModified(handlerId, dieuPhoiDate);
        }

        // Step 3: Báo giá
        if (target.Value >= TrangThaiSuaChua.DaDuyetBaoGia.Value && target != TrangThaiSuaChua.DaHuy)
        {
            var baoGiaDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(4, 24));
            bool isFree = request.HopDongDoiTacId != null || faker.Random.Bool(0.3f);
            decimal cost = isFree ? 0 : faker.Finance.Amount(50000, 500000, 0);

            request.NhapBaoGia(cost, isFree, isFree ? "Hạng mục nằm trong gói bảo trì." : "Chi phí thay thế linh kiện chính hãng.");
            request.SetModified(handlerId, baoGiaDate);

            if (!isFree)
            {
                request.CuDanDuyetBaoGia();
                request.SetModified(request.CreatedBy, baoGiaDate.AddHours(faker.Random.Int(1, 12)));
            }
        }

        // Step 4: Thực hiện
        if (target.Value >= TrangThaiSuaChua.DangXuLy.Value && target != TrangThaiSuaChua.DaHuy)
        {
            var scheduleDate = (request.ModifiedAt ?? request.CreatedAt).AddDays(faker.Random.Int(1, 3));
            request.HenLich(scheduleDate, scheduleDate.AddHours(2));
            request.BatDauXuLy();
            request.SetModified(handlerId, scheduleDate.AddMinutes(30));
        }

        // Step 5: Hoàn tất
        if (target.Value >= TrangThaiSuaChua.DaXuLy.Value && target != TrangThaiSuaChua.DaHuy)
        {
            var hoanTatDate = (request.ModifiedAt ?? request.CreatedAt).AddHours(faker.Random.Int(1, 4));
            var actualCost = request.ChiPhiDuKien;
            request.HoanTatXuLy("Đã xử lý dứt điểm sự cố, khách hàng hài lòng.", actualCost, hoanTatDate);
        }

        // Step 6: Đóng
        if (target == TrangThaiSuaChua.DaDong)
        {
            var dongDate = (request.ModifiedAt ?? request.CreatedAt).AddDays(faker.Random.Int(1, 2));
            request.DongYeuCau();
            request.SetModified(request.CreatedBy, dongDate);
        }

        // Step 7: Hủy
        if (target == TrangThaiSuaChua.DaHuy)
        {
            request.Huy(faker.PickRandom(new[] { "Khách hàng đổi ý, tự sửa chữa.", "Không liên lạc được với khách hàng.", "Hạng mục không nằm trong phạm vi hỗ trợ." }));
            request.SetModified(handlerId, createdDate.AddHours(faker.Random.Int(1, 48)));
        }
    }
}
