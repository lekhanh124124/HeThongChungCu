using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class BangGiaSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.BangGias.AnyAsync())
        {
            logger.LogInformation("Seeding BangGias...");

            var dichVus = await context.Set<DichVu>().ToListAsync();
            var ngayApDung = new DateTime(2024, 1, 1);

            var bangGias = new List<BangGia>();

            foreach (var dv in dichVus)
            {
                if (dv.MaDichVu == "DV-DIEN")
                {
                    var bgDien = new BangGia(dv.Id, "Bảng giá điện sinh hoạt 2024", ngayApDung, LoaiDinhGia.LuyTien);
                    bgDien.AddLuyTien(0, 50, 1678);
                    bgDien.AddLuyTien(50, 100, 1734);
                    bgDien.AddLuyTien(100, 200, 2014);
                    bgDien.AddLuyTien(200, 300, 2536);
                    bgDien.AddLuyTien(300, 400, 2834);
                    bgDien.AddLuyTien(400, null, 2927);
                    bangGias.Add(bgDien);
                }
                else if (dv.MaDichVu == "DV-NUOC")
                {
                    var bgNuoc = new BangGia(dv.Id, "Bảng giá nước sinh hoạt 2024", ngayApDung, LoaiDinhGia.LuyTien);
                    bgNuoc.AddLuyTien(0, 10, 5973);
                    bgNuoc.AddLuyTien(10, 20, 7052);
                    bgNuoc.AddLuyTien(20, 30, 8669);
                    bgNuoc.AddLuyTien(30, null, 15929);
                    bangGias.Add(bgNuoc);
                }
                else if (dv.MaDichVu == "DV-QL")
                {
                    bangGias.Add(new BangGia(dv.Id, "Phí quản lý chung cư", ngayApDung, LoaiDinhGia.CoDinh, 10000));
                }
                else if (dv.MaDichVu == "DV-GUIXE")
                {
                    bangGias.Add(new BangGia(dv.Id, "Phí gửi xe máy", ngayApDung, LoaiDinhGia.CoDinh, 100000));
                }
                else if (dv.MaDichVu == "DV-RAC")
                {
                    bangGias.Add(new BangGia(dv.Id, "Phí thu gom rác", ngayApDung, LoaiDinhGia.CoDinh, 30000));
                }
                else
                {
                    bangGias.Add(new BangGia(dv.Id, $"Bảng giá {dv.TenDichVu}", ngayApDung, LoaiDinhGia.CoDinh, 50000));
                }
            }

            await context.BangGias.AddRangeAsync(bangGias);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} BangGias.", bangGias.Count);
        }
    }
}
