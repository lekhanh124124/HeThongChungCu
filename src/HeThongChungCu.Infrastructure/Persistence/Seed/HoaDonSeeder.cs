using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

using System.Text.Json;
using Bogus;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public static class HoaDonSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.HoaDons.AnyAsync())
        {
            logger.LogInformation("Invoices already seeded. Skipping HoaDonSeeder.");
            return;
        }

        logger.LogInformation("Seeding Invoices and Payments from Jan 2025 to May 2026 for ALL active apartments...");

        var faker = new Faker("vi");

        // 1. Lấy dữ liệu nền tảng
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 1;

        // Lấy tất cả các ID căn hộ đang cư trú
        var activeApartmentIds = await context.QuanHeCuTrus
            .Where(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru && 
                        (r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue))
            .Select(r => r.CanHoId)
            .Distinct()
            .ToListAsync();

        if (!activeApartmentIds.Any())
        {
            logger.LogWarning("No active apartments found. Skipping invoice seeding.");
            return;
        }

        var activeApartments = await context.CanHos
            .Where(c => activeApartmentIds.Contains(c.Id))
            .ToListAsync();

        // Map căn hộ -> Người dùng ID để gán cho phiên thanh toán
        var relationList = await context.QuanHeCuTrus
            .Where(r => activeApartmentIds.Contains(r.CanHoId) && 
                        (r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue))
            .Select(r => new { r.CanHoId, r.NguoiDungId })
            .ToListAsync();
            
        var relationDict = relationList
            .GroupBy(r => r.CanHoId)
            .ToDictionary(g => g.Key, g => g.First().NguoiDungId);

        var dvQuanLy = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "MANAGEMENT_FEE");
        var dvDien = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "ELECTRICITY");
        var dvNuoc = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "WATER");
        var dvLaiTreHan = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "LATE_INTEREST_FEE");
        var dvMotor = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "PK_MOTOR");
        var dvCar = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "PK_CAR");
        var dvRac = await context.DichVus.FirstOrDefaultAsync(d => d.MaDichVu == "DV_RAC");

        // Khởi tạo chỉ số điện nước gốc ngẫu nhiên cho từng căn hộ
        var currentDien = activeApartmentIds.ToDictionary(id => id, id => faker.Random.Int(50, 200));
        var currentNuoc = activeApartmentIds.ToDictionary(id => id, id => faker.Random.Int(10, 50));

        var listSuaChuaCompleted = await context.YeuCauSuaChuas
            .Where(s => s.TrangThaiId == TrangThaiYeuCau.Completed)
            .ToListAsync();

        var listThiCongChoCoc = await context.YeuCauThiCongs
            .Where(t => t.TrangThaiId == TrangThaiYeuCau.Approved)
            .ToListAsync();

        int invoiceCodeCounter = 1000;

        // VÒNG LẶP THỜI GIAN (01/2025 -> 05/2026)
        for (int year = 2025; year <= 2026; year++)
        {
            int endMonth = year == 2026 ? 5 : 12;
            for (int month = 1; month <= endMonth; month++)
            {
                // 2. Tạo Đợt thanh toán (DotThanhToan)
                var dotThanhToan = DotThanhToan.Create($"Đợt thanh toán {month}/{year}", new KyThanhToan(month, year), $"Đợt thu phí định kỳ tháng {month} năm {year}").Value;
                
                if (year == 2026 && month == 5)
                {
                    // Bản nháp (Không phát hành) cho tháng 5/2026 theo yêu cầu
                    dotThanhToan.MarkAsApproved();
                    dotThanhToan.MarkAsDraftGenerated();
                }
                else
                {
                    dotThanhToan.MarkAsApproved();
                    dotThanhToan.MarkAsDraftGenerated();
                    dotThanhToan.MarkAsIssued();
                }

                await context.DotThanhToan.AddAsync(dotThanhToan);
                DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();

                // VÒNG LẶP CĂN HỘ
                foreach (var canHo in activeApartments)
                {
                    invoiceCodeCounter++;
                    var maHD = $"HD-{year}{month:D2}-{canHo.MaCanHo}";
                    var ngayLap = new DateTimeOffset(year, month, 5, 8, 0, 0, TimeSpan.FromHours(7));
                    var ngayHan = new DateTimeOffset(year, month, 20, 23, 59, 59, TimeSpan.FromHours(7));
                    
                    // Xác định trạng thái của hóa đơn
                    TrangThaiHoaDon targetStatus;
                    if (year == 2026 && month == 5)
                    {
                        targetStatus = TrangThaiHoaDon.ChoDuyet; // Bản nháp
                    }
                    else if (year == 2026 && month == 4)
                    {
                        var rand = faker.Random.Int(1, 100);
                        if (rand <= 80) targetStatus = TrangThaiHoaDon.DaThanhToan;
                        else if (rand <= 90) targetStatus = TrangThaiHoaDon.ThanhToanMotPhan;
                        else targetStatus = TrangThaiHoaDon.ChuaThanhToan;
                    }
                    else
                    {
                        var rand = faker.Random.Int(1, 100);
                        if (rand <= 95) targetStatus = TrangThaiHoaDon.DaThanhToan;
                        else targetStatus = TrangThaiHoaDon.QuaHan;
                    }

                    var hdRes = HoaDon.CreateHoaDon(canHo.Id, dotThanhToan.Id, maHD, new KyThanhToan(month, year), ngayLap, ngayHan, $"Hóa đơn dịch vụ Tháng {month}/{year}");

                    if (hdRes.IsSuccess)
                    {
                        var hd = hdRes.Value;
                        if (dvQuanLy != null) hd.AddDichVuDetail("Phí quản lý vận hành căn hộ", 1, canHo.ThongSo.DienTich * 10000, dvQuanLy.Id, $"Tính theo diện tích {canHo.ThongSo.DienTich} m2");
                        
                        var tieuThuDien = faker.Random.Int(100, 300);
                        var chiSoMoiDien = currentDien[canHo.Id] + tieuThuDien;
                        if (dvDien != null) hd.AddTieuThuDetail("Tiền điện sinh hoạt lũy tiến", currentDien[canHo.Id], chiSoMoiDien, 2167, dvDien.Id, $"Tiêu thụ {tieuThuDien} kWh");
                        currentDien[canHo.Id] = chiSoMoiDien;

                        var tieuThuNuoc = faker.Random.Int(10, 30);
                        var chiSoMoiNuoc = currentNuoc[canHo.Id] + tieuThuNuoc;
                        if (dvNuoc != null) hd.AddTieuThuDetail("Tiền nước sạch sinh hoạt", currentNuoc[canHo.Id], chiSoMoiNuoc, 7052, dvNuoc.Id, $"Tiêu thụ {tieuThuNuoc} m3");
                        currentNuoc[canHo.Id] = chiSoMoiNuoc;

                        var numBikes = faker.Random.Int(1, 2); // giả lập ai cũng có xe máy
                        if (dvMotor != null) hd.AddDichVuDetail($"Phí giữ xe máy ({numBikes} xe)", numBikes, 100000, dvMotor.Id);
                        
                        var hasCar = faker.Random.Bool(0.3f); // 30% có ô tô
                        if (dvCar != null && hasCar) hd.AddDichVuDetail($"Phí giữ xe ô tô (1 xe)", 1, 1200000, dvCar.Id);
                        
                        if (dvRac != null) hd.AddDichVuDetail("Phí thu gom rác thải căn hộ", 1, 30000, dvRac.Id);

                        // Gán phí thi công hoặc sửa chữa ngẫu nhiên vào tháng bất kỳ
                        if (faker.Random.Bool(0.01f)) // 1% xác suất
                        {
                            var suaChua = listSuaChuaCompleted.FirstOrDefault(s => s.CanHoId == canHo.Id && s.ChiPhiThucTe > 0);
                            if (suaChua != null) hd.AddSuaChuaDetail(suaChua.Id, $"Phí sửa chữa thực tế - {suaChua.NoiDung}", suaChua.ChiPhiThucTe ?? 120000);
                        }

                        if (faker.Random.Bool(0.01f)) // 1% xác suất
                        {
                            var thiCong = listThiCongChoCoc.FirstOrDefault(t => t.CanHoId == canHo.Id);
                            if (thiCong != null) hd.AddThiCongDetail(thiCong.Id, LoaiChiPhiThiCong.DatCoc, $"Tiền ký quỹ đặt cọc - {thiCong.HangMucThiCong}", thiCong.TienDatCoc ?? 10000000);
                        }

                        // Cập nhật trạng thái
                        if (targetStatus != TrangThaiHoaDon.ChoDuyet) 
                        {
                            hd.UpdateStatus(targetStatus);
                        }

                        if (targetStatus == TrangThaiHoaDon.QuaHan)
                        {
                            hd.SetNgayTinhLai(ngayLap.AddMonths(1));
                        }

                        await context.HoaDons.AddAsync(hd);
                        // Cho phép domain events AutoRecordIncomeOnPaymentHandler kích hoạt
                        await context.SaveChangesAsync();

                        var nguoiDungId = relationDict.ContainsKey(canHo.Id) ? relationDict[canHo.Id] : adminId;

                        // Tạo giao dịch thanh toán
                        if (targetStatus == TrangThaiHoaDon.DaThanhToan || targetStatus == TrangThaiHoaDon.ThanhToanMotPhan)
                        {
                            var payDate = ngayLap.AddDays(faker.Random.Int(1, 14));
                            decimal totalPaid = 0;
                            var paidIds = new List<int>();

                            var detailsToPay = targetStatus == TrangThaiHoaDon.DaThanhToan 
                                ? hd.ChiTietHoaDons.Where(d => d.ThanhTien > 0).ToList()
                                : hd.ChiTietHoaDons.Where(d => (d.TenMucPhi.ToLower().Contains("điện") || d.TenMucPhi.ToLower().Contains("nước")) && d.ThanhTien > 0).ToList();

                            foreach (var detail in detailsToPay)
                            {
                                var gd = GiaoDichThanhToan.RecordTransaction(detail.Id, detail.ThanhTien, PhuongThucThanhToan.ChuyenKhoan, $"FT{year}{month:D2}{detail.Id:D5}", "Thanh toán dịch vụ tự động").Value;
                                if (adminId != 0) gd.SetCreated(adminId, payDate);
                                await context.GiaoDichThanhToans.AddAsync(gd);
                                totalPaid += detail.ThanhTien;
                                paidIds.Add(detail.Id);
                            }

                            if (paidIds.Any())
                            {
                                var listIds = string.Join(",", paidIds);
                                var phien = new PhienThanhToan($"TXN_{month}_{year}_{hd.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", hd.Id, listIds, totalPaid, targetStatus == TrangThaiHoaDon.DaThanhToan ? "Thanh toán đầy đủ" : "Thanh toán một phần");
                                phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
                                if (adminId != 0) phien.SetCreated(adminId, payDate);
                                await context.PhienThanhToans.AddAsync(phien);
                            }
                            
                            // Lưu thay đổi sau khi tạo giao dịch để đảm bảo nhất quán
                            await context.SaveChangesAsync();
                        }

                        // Sinh ChiSoTieuThu
                        foreach (var detail in hd.ChiTietHoaDons)
                        {
                            if (detail is ChiTietHoaDonTieuThu tieuThuDetail)
                            {
                                var cs = ChiSoTieuThu.Create(
                                    hd.CanHoId,
                                    tieuThuDetail.DichVuId,
                                    tieuThuDetail.ChiSoCu,
                                    tieuThuDetail.ChiSoMoi,
                                    hd.KyThanhToan.Thang,
                                    hd.KyThanhToan.Nam,
                                    hd.NgayLap,
                                    null,
                                    tieuThuDetail.GhiChu,
                                    null);

                                cs.Confirm();
                                cs.MarkAsBilled(hd.Id);
                                await context.ChiSoTieuThus.AddAsync(cs);
                            }
                        }
                    }
                }
                
                // Clear domain events and save ChiSoTieuThu
                DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();
                
                logger.LogInformation($"Finished seeding Billing Period {month}/{year}. Generated {activeApartments.Count} invoices.");
            }
        }

        logger.LogInformation("Database Seeding for Invoices and Payments from Jan 2025 to May 2026 completed successfully!");
    }
}
