using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class YeuCauCuTruSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        YeuCauCounts? counts)
    {
        if (counts == null) return;

        logger.LogInformation("Seeding YeuCauCuTru...");

        var faker = new Faker("vi");
        var adminAccount = await context.TaiKhoan
            .FirstOrDefaultAsync(a => a.TenDangNhap == "admin@gmail.com");

        // Get all residents grouped by apartment
        var allResidents = await (from qh in context.QuanHeCuTrus
                                  join tk in context.TaiKhoan on qh.NguoiDungId equals tk.NguoiDungId into tkGroup
                                  from tk in tkGroup.DefaultIfEmpty()
                                  join u in context.NguoiDung on qh.NguoiDungId equals u.Id
                                  select new ResidentData
                                  {
                                      Id = qh.Id,
                                      CanHoId = qh.CanHoId,
                                      TaiKhoanId = tk != null ? tk.Id : (int?)null,
                                      TrangThaiCuTruId = qh.TrangThaiCuTruId,
                                      LoaiQuanHeCuTruId = qh.LoaiQuanHeCuTruId.Value,
                                      GioiTinhId = u.GioiTinhId.Value
                                  }).ToListAsync();

        var residentsByApartment = allResidents
            .GroupBy(r => r.CanHoId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var apartmentsWithChuHo = residentsByApartment
            .Where(g => g.Value.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo.Value && r.TaiKhoanId.HasValue))
            .ToList();

        if (apartmentsWithChuHo.Count == 0)
        {
            logger.LogWarning("No apartments with active ChuHo found. Skipping YeuCauCuTru seeding.");
            return;
        }

        await SeedResidencyRequestsByType(context, apartmentsWithChuHo, LoaiHanhDongYeuCau.Them, counts.SoLuongThem, faker, adminAccount);
        await SeedResidencyRequestsByType(context, apartmentsWithChuHo, LoaiHanhDongYeuCau.Sua, counts.SoLuongSua, faker, adminAccount);
        await SeedResidencyRequestsByType(context, apartmentsWithChuHo, LoaiHanhDongYeuCau.Xoa, counts.SoLuongXoa, faker, adminAccount);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding YeuCauCuTru.");
    }

    private static async Task SeedResidencyRequestsByType(
        AppDbContext context,
        List<KeyValuePair<int, List<ResidentData>>> apartments,
        LoaiHanhDongYeuCau loaiYeuCau,
        int count,
        Faker faker,
        TaiKhoan? admin)
    {
        for (int i = 0; i < count; i++)
        {
            var aptEntry = faker.PickRandom(apartments);
            var residents = aptEntry.Value;
            var chuHo = residents.First(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo.Value);
            
            var initialStatus = DetermineInitialStatus(chuHo, faker, out var targetStatus);

            YeuCauCuTru request;
            if (loaiYeuCau == LoaiHanhDongYeuCau.Them)
            {
                var addContents = new[]
                {
                    "Đăng ký tạm trú cho người thân vừa chuyển đến từ quê.",
                    "Bổ sung thành viên mới vào sổ hộ khẩu gia đình (con mới sinh).",
                    "Đăng ký cư trú cho người giúp việc theo hợp đồng mới.",
                    "Đăng ký cho bố mẹ lên ở cùng để tiện chăm sóc sức khỏe.",
                    "Đăng ký tạm trú cho em gái lên học đại học và ở cùng anh chị.",
                    "Bổ sung thông tin vợ mới cưới vào danh sách cư dân căn hộ."
                };
                var dob = faker.Date.Past(30, DateTime.Now.AddYears(-20));
                var genderId = faker.PickRandom(new[] { 1, 2 });
                request = YeuCauCuTru.CreateAddMemberRequest(
                    aptEntry.Key,
                    null,
                    LoaiQuanHeCuTru.NguoiOCung.Value,
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    dob,
                    genderId,
                    UserSeeder.GetUniquePhoneNumber(),
                    UserSeeder.GetUniqueIdCard(genderId, dob.Year),
                    UserSeeder.GetRandomVietnamAddress(),
                    faker.PickRandom(addContents),
                null,
                initialStatus);
            }
            else if (loaiYeuCau == LoaiHanhDongYeuCau.Sua)
            {
                // Pick any resident in this apartment to update
                var target = faker.PickRandom(residents);
                var updateContents = new[]
                {
                    "Cập nhật lại số điện thoại liên lạc chính xác do thay đổi SIM.",
                    "Sửa đổi thông tin nghề nghiệp và nơi làm việc hiện tại.",
                    "Cập nhật ảnh thẻ cư dân mới để làm lại thẻ từ thang máy.",
                    "Đính chính lại sai sót về ngày tháng năm sinh trong hồ sơ.",
                    "Cập nhật số CCCD mới sau khi làm lại thẻ căn cước có gắn chip."
                };
                var dobUpdate = faker.Date.Past(25, DateTime.Now.AddYears(-18));
                request = YeuCauCuTru.CreateUpdateMemberRequest(
                    aptEntry.Key,
                    target.Id,
                    target.LoaiQuanHeCuTruId, 
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    dobUpdate,
                    target.GioiTinhId,
                    UserSeeder.GetUniquePhoneNumber(),
                    UserSeeder.GetUniqueIdCard(target.GioiTinhId, dobUpdate.Year),
                    UserSeeder.GetRandomVietnamAddress(),
                    faker.PickRandom(updateContents),
                null,
                initialStatus);
            }
            else // Xoa
            {
                // Try to find a non-ChuHo member to remove
                var nonChuHoMembers = residents.Where(r => r.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo.Value).ToList();
                if (nonChuHoMembers.Count == 0) continue; // Skip if only ChuHo exists

                var targetToRemove = faker.PickRandom(nonChuHoMembers);
                var removeContents = new[]
                {
                    "Thành viên gia đình đã chuyển đi nơi khác sinh sống.",
                    "Người thuê đã hết hạn hợp đồng thuê nhà và trả phòng.",
                    "Hủy thông tin đăng ký tạm trú cho khách ở chơi dài ngày.",
                    "Xóa thông tin người giúp việc cũ đã nghỉ việc.",
                    "Thành viên chuyển đi du học nước ngoài dài hạn."
                };
                request = YeuCauCuTru.CreateRemoveMemberRequest(
                    aptEntry.Key,
                    targetToRemove.Id,
                    faker.PickRandom(removeContents),
                    initialStatus);
            }

            // Requester is the Chu Ho
            request.SetCreated(chuHo.TaiKhoanId!.Value, DateTimeOffset.Now.AddDays(-faker.Random.Number(5, 10)));

            // Apply Approval/Rejection if needed
            if (targetStatus == TrangThaiYeuCau.Approved && admin != null)
            {
                request.Approve(admin.Id, DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }
            else if (targetStatus == TrangThaiYeuCau.Rejected && admin != null)
            {
                var rejectionReasons = new[]
                {
                    "Hồ sơ đính kèm không đủ cơ sở pháp lý (thiếu giấy tạm trú).",
                    "Ảnh chụp giấy tờ tùy thân bị mờ, không nhìn rõ thông tin.",
                    "Căn hộ đã đạt số lượng cư dân tối đa theo diện tích.",
                    "Thông tin khai báo không khớp với dữ liệu dân cư phường."
                };
                request.Reject(admin.Id, faker.PickRandom(rejectionReasons), DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 5)));
            }

            await context.YeuCauCuTrus.AddAsync(request);
        }
    }

    private static TrangThaiYeuCau DetermineInitialStatus(ResidentData resident, Faker faker, out TrangThaiYeuCau targetStatus)
    {
        if (resident.TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
        {
            targetStatus = TrangThaiYeuCau.Invalidated;
            return TrangThaiYeuCau.Invalidated;
        }

        // 60% Approved, 20% Pending, 10% Rejected, 5% Saved, 5% Withdrawn
        var rand = faker.Random.Number(1, 100);

        if (rand <= 60)
        {
            targetStatus = TrangThaiYeuCau.Approved;
            return TrangThaiYeuCau.Pending; // Needs to be Pending to call Approve()
        }

        if (rand <= 80)
        {
            targetStatus = TrangThaiYeuCau.Pending;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 90)
        {
            targetStatus = TrangThaiYeuCau.Rejected;
            return TrangThaiYeuCau.Pending; // Needs to be Pending to call Reject()
        }

        if (rand <= 95)
        {
            targetStatus = TrangThaiYeuCau.Saved;
            return TrangThaiYeuCau.Saved;
        }

        targetStatus = TrangThaiYeuCau.Withdrawn;
        return TrangThaiYeuCau.Saved; // Can be withdrawn from Saved
    }
    private class ResidentData
    {
        public int Id { get; set; }
        public int CanHoId { get; set; }
        public int? TaiKhoanId { get; set; }
        public TrangThaiCuTru TrangThaiCuTruId { get; set; } = null!;
        public int LoaiQuanHeCuTruId { get; set; }
        public int GioiTinhId { get; set; }
    }
}
