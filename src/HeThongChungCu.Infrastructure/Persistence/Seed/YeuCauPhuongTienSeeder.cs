using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class YeuCauPhuongTienSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        YeuCauCounts? counts)
    {
        if (counts == null) return;

        logger.LogInformation("Seeding YeuCauPhuongTien...");

        var faker = new Faker("vi");
        var adminAccount = await context.TaiKhoan
            .FirstOrDefaultAsync(a => a.TenDangNhap == "admin@gmail.com");

        // Get all vehicles grouped by apartment
        var vehiclesByApartment = await context.PhuongTiens
            .GroupBy(v => v.CanHoId)
            .ToDictionaryAsync(g => g.Key, g => g.ToList());

        // Get householders with their TaiKhoanId
        var householders = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue)
            .Join(context.TaiKhoan,
                qh => qh.NguoiDungId,
                tk => tk.NguoiDungId,
                (qh, tk) => new HouseholderData
                {
                    Id = qh.Id,
                    CanHoId = qh.CanHoId,
                    TaiKhoanId = tk.Id,
                    TrangThaiCuTruId = qh.TrangThaiCuTruId,
                    NgayBatDau = qh.ThoiGian.NgayBatDau,
                    NgayKetThuc = qh.ThoiGian.NgayKetThuc
                })
            .ToListAsync();

        if (householders.Count == 0)
        {
            logger.LogWarning("No householders found. Skipping YeuCauPhuongTien seeding.");
            return;
        }

        await SeedVehicleRequestsByType(context, householders, vehiclesByApartment, LoaiHanhDongYeuCau.Them, counts.SoLuongThem, faker, adminAccount);
        await SeedVehicleRequestsByType(context, householders, vehiclesByApartment, LoaiHanhDongYeuCau.Sua, counts.SoLuongSua, faker, adminAccount);
        await SeedVehicleRequestsByType(context, householders, vehiclesByApartment, LoaiHanhDongYeuCau.Xoa, counts.SoLuongXoa, faker, adminAccount);

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding YeuCauPhuongTien.");
    }

    private static async Task SeedVehicleRequestsByType(
        AppDbContext context,
        List<HouseholderData> householders,
        Dictionary<int, List<PhuongTien>> vehiclesByApartment,
        LoaiHanhDongYeuCau loaiYeuCau,
        int count,
        Faker faker,
        TaiKhoan? admin)
    {
        var motorbikeTypes = new[] { LoaiPhuongTien.XeMay, LoaiPhuongTien.Oto, LoaiPhuongTien.XeDap, LoaiPhuongTien.XeDien };

        for (int i = 0; i < count; i++)
        {
            var householder = faker.PickRandom(householders);
            var initialStatus = DetermineInitialStatus(householder, faker, out var targetStatus);
            var loaiXe = faker.PickRandom(motorbikeTypes);

            YeuCauPhuongTien request;
            if (loaiYeuCau == LoaiHanhDongYeuCau.Them)
            {
                var addContents = new[]
                {
                    "Đăng ký xe mới mua, loại sedan 5 chỗ để đi làm.",
                    "Đăng ký thêm thẻ gửi xe máy cho con mới đi học đại học.",
                    "Đăng ký chỗ đậu xe ô tô cố định dưới hầm B1.",
                    "Đăng ký sạc điện cho xe máy điện mới mua, cần vị trí gần trạm sạc.",
                    "Đăng ký xe đạp điện mới để đưa đón con đi học.",
                    "Bổ sung xe ô tô thứ 2 cho gia đình (xe SUV 7 chỗ)."
                };
                request = YeuCauPhuongTien.CreateAddRequest(
                    householder.CanHoId,
                    loaiXe,
                    faker.Vehicle.Model(),
                    faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                    faker.Commerce.Color(),
                    faker.PickRandom(addContents),
                    null,
                    initialStatus);
            }
            else
            {
                // For Update/Delete, find a vehicle from the requester's apartment
                if (!vehiclesByApartment.TryGetValue(householder.CanHoId, out var apartmentVehicles) || apartmentVehicles.Count == 0)
                {
                    // If no vehicles, try to pick another householder who HAS vehicles
                    var possibleHouseholders = householders
                        .Where(h => vehiclesByApartment.ContainsKey(h.CanHoId) && vehiclesByApartment[h.CanHoId].Count > 0)
                        .ToList();

                    if (possibleHouseholders.Count == 0) continue;
                    
                    householder = faker.PickRandom(possibleHouseholders);
                    apartmentVehicles = vehiclesByApartment[householder.CanHoId];
                }

                var targetVehicle = faker.PickRandom(apartmentVehicles);

                if (loaiYeuCau == LoaiHanhDongYeuCau.Sua)
                {
                    var updateContents = new[]
                    {
                        "Cập nhật lại biển số xe mới sau khi làm thủ tục sang tên đổi chủ.",
                        "Sửa đổi thông tin màu sơn xe thực tế (đã dán decal đổi màu).",
                        "Cập nhật dòng xe chính xác hơn theo giấy tờ đăng ký xe.",
                        "Đính chính lại số khung, số máy do bị nhầm lẫn khi đăng ký lần đầu.",
                        "Chuyển đổi từ xe xăng sang xe điện, cần đăng ký lại dịch vụ sạc."
                    };
                    request = YeuCauPhuongTien.CreateUpdateRequest(
                        householder.CanHoId,
                        targetVehicle.Id,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.PickRandom(updateContents),
                        null,
                        initialStatus);
                }
                else // Xoa
                {
                    var removeContents = new[]
                    {
                        "Hủy thẻ gửi xe do đã bán phương tiện cho người khác.",
                        "Hết nhu cầu gửi xe ô tô tại chung cư do đã có chỗ gửi ngoài.",
                        "Xóa thông tin xe máy cũ đã hư hỏng, không còn sử dụng.",
                        "Hủy dịch vụ sạc xe điện do đã thanh lý xe.",
                        "Gia đình chuyển nhà đi nơi khác, cần hủy toàn bộ thẻ xe."
                    };
                    request = YeuCauPhuongTien.CreateDeleteRequest(
                        householder.CanHoId,
                        targetVehicle.Id,
                        loaiXe,
                        faker.Vehicle.Model(),
                        faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                        faker.Commerce.Color(),
                        faker.PickRandom(removeContents),
                        initialStatus);
                }
            }

            var minDate = householder.NgayBatDau;
            var maxDate = householder.NgayKetThuc ?? DateTimeOffset.Now;
            if (minDate >= maxDate) minDate = maxDate.AddDays(-1);
            var createdDate = minDate.AddDays(faker.Random.Number(0, (int)(maxDate - minDate).TotalDays));

            // Set the requester (CreatedBy) manually for seed data
            request.SetCreated(householder.TaiKhoanId, createdDate);

            // Apply Approval/Rejection/Return/Invalidation if needed
            if (admin != null)
            {
                if (targetStatus == TrangThaiYeuCau.Approved)
                {
                    request.Approve(admin.Id, DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 4)));

                    // PHYSICAL SIDE EFFECTS for Approved Requests
                    if (loaiYeuCau == LoaiHanhDongYeuCau.Them)
                    {
                        var pt = new PhuongTien(
                            request.CanHoId,
                            request.YeuCauTenPhuongTien,
                            request.YeuCauLoaiPhuongTienId,
                            PhuongTienSeeder.RegisterBienSo(request.YeuCauBienSo),
                            request.YeuCauMauXe
                        );
                        pt.SetCreated(admin.Id, request.NgayXuLy ?? DateTimeOffset.Now);
                        await context.PhuongTiens.AddAsync(pt);
                        await context.SaveChangesAsync(); // Need ID

                        // Set the resulting PT ID back to the request
                        var ptIdField = typeof(YeuCauPhuongTien).GetProperty("YeuCauPhuongTienId");
                        ptIdField?.SetValue(request, pt.Id);

                        // Add a card for the new vehicle
                        var the = pt.AddThe(PhuongTienSeeder.GenerateUniqueMaThe(faker), pt.CreatedAt);
                        the.SetCreated(admin.Id, pt.CreatedAt);
                    }
                    else if (loaiYeuCau == LoaiHanhDongYeuCau.Sua && request.YeuCauPhuongTienId.HasValue)
                    {
                        var pt = await context.PhuongTiens.FindAsync(request.YeuCauPhuongTienId.Value);
                        if (pt != null)
                        {
                            pt.CapNhat(
                                request.YeuCauTenPhuongTien,
                                request.YeuCauLoaiPhuongTienId,
                                PhuongTienSeeder.RegisterBienSo(request.YeuCauBienSo),
                                request.YeuCauMauXe
                            );
                            pt.SetModified(admin.Id, request.NgayXuLy ?? DateTimeOffset.Now);
                        }
                    }
                    else if (loaiYeuCau == LoaiHanhDongYeuCau.Xoa && request.YeuCauPhuongTienId.HasValue)
                    {
                        var pt = await context.PhuongTiens.FindAsync(request.YeuCauPhuongTienId.Value);
                        if (pt != null)
                        {
                            pt.Huy(request.NgayXuLy ?? DateTimeOffset.Now);
                            pt.SetModified(admin.Id, request.NgayXuLy ?? DateTimeOffset.Now);
                        }
                    }
                }
                else if (targetStatus == TrangThaiYeuCau.Rejected)
                {
                    var rejectionReasons = new[]
                    {
                        "Biển số xe không rõ ràng hoặc hình ảnh cung cấp bị lóa mờ.",
                        "Vượt quá số lượng phương tiện tối đa cho phép của một căn hộ.",
                        "Loại xe không được phép gửi trong hầm tòa nhà theo quy định.",
                        "Giấy tờ xe (Cavet) không chính chủ hoặc thiếu thông tin hợp lệ.",
                        "Biển số xe đã được đăng ký cho một căn hộ khác trong hệ thống."
                    };
                    request.Reject(admin.Id, faker.PickRandom(rejectionReasons), DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 4)));
                }
                else if (targetStatus == TrangThaiYeuCau.Returned)
                {
                    var returnReasons = new[]
                    {
                        "Vui lòng bổ sung ảnh chụp giấy đăng ký xe (Cavet) rõ nét.",
                        "Cần cung cấp ảnh chụp mặt trước và mặt sau của phương tiện.",
                        "Thông tin biển số xe không khớp với hình ảnh đính kèm.",
                        "Vui lòng đính chính lại số khung/số máy theo đúng giấy tờ."
                    };
                    request.Return(admin.Id, faker.PickRandom(returnReasons), DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 4)));
                }
                else if (targetStatus == TrangThaiYeuCau.Invalidated)
                {
                    request.Invalidate(admin.Id, "Cư dân đã kết thúc cư trú hoặc không còn sử dụng phương tiện này.", DateTimeOffset.Now.AddDays(-faker.Random.Number(1, 4)));
                }
            }

            await context.YeuCauPhuongTiens.AddAsync(request);
        }
    }

    private static TrangThaiYeuCau DetermineInitialStatus(HouseholderData householder, Faker faker, out TrangThaiYeuCau targetStatus)
    {
        if (householder.TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
        {
            targetStatus = TrangThaiYeuCau.Invalidated;
            return TrangThaiYeuCau.Pending;
        }

        // 55% Approved, 15% Pending, 10% Rejected, 10% Returned, 5% Saved, 5% Withdrawn
        var rand = faker.Random.Number(1, 100);

        if (rand <= 55)
        {
            targetStatus = TrangThaiYeuCau.Approved;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 70)
        {
            targetStatus = TrangThaiYeuCau.Pending;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 80)
        {
            targetStatus = TrangThaiYeuCau.Rejected;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 90)
        {
            targetStatus = TrangThaiYeuCau.Returned;
            return TrangThaiYeuCau.Pending;
        }

        if (rand <= 95)
        {
            targetStatus = TrangThaiYeuCau.Saved;
            return TrangThaiYeuCau.Saved;
        }

        targetStatus = TrangThaiYeuCau.Withdrawn;
        return TrangThaiYeuCau.Saved;
    }
    private class HouseholderData
    {
        public int Id { get; set; }
        public int CanHoId { get; set; }
        public int TaiKhoanId { get; set; }
        public TrangThaiCuTru TrangThaiCuTruId { get; set; } = null!;
        public DateTimeOffset NgayBatDau { get; set; }
        public DateTimeOffset? NgayKetThuc { get; set; }
    }
}
