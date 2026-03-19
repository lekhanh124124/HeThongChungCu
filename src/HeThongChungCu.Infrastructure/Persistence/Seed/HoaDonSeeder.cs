using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class HoaDonSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.HoaDons.AnyAsync())
        {
            logger.LogInformation("Seeding HoaDons...");

            var canHos = await context.CanHos.Take(10).ToListAsync();
            var bangGias = await context.BangGias.Include(x => x.BangGiaLuyTiens).ToListAsync();
            var dichVus = await context.Set<DichVu>().ToListAsync();
            var chiSos = await context.Set<ChiSoTieuThu>().ToListAsync();

            var hoaDons = new List<HoaDon>();

            foreach (var canHo in canHos)
            {
                // Seed for month 1, 2, 3
                for (int month = 1; month <= 3; month++)
                {
                    var maHoaDon = $"HD-{canHo.MaCanHo}-{month:D2}-2024";
                    var hanThanhToan = new DateTime(2024, month, 15).AddMonths(1);
                    var hoaDon = new HoaDon(maHoaDon, canHo.Id, month, 2024, hanThanhToan, $"Tiền điện nước tháng {month}");

                    // Find chi so tieu thu for this apartment and month
                    var chiSoCanHo = chiSos.Where(x => x.CanHoId == canHo.Id && x.Thang == month && x.Nam == 2024).ToList();

                    foreach (var cs in chiSoCanHo)
                    {
                        var dv = dichVus.First(x => x.Id == cs.DichVuId);
                        var bg = bangGias.First(x => x.DichVuId == cs.DichVuId);

                        if (bg.LoaiDinhGiaId == LoaiDinhGia.LuyTien)
                        {
                            // Calculate tiered price
                            decimal totalAmount = 0;
                            double remainingQty = cs.SoLuong;
                            
                            foreach (var tier in bg.BangGiaLuyTiens.OrderBy(x => x.TuMuc))
                            {
                                if (remainingQty <= 0) break;
                                
                                double tierRange = (tier.DenMuc ?? double.MaxValue) - tier.TuMuc;
                                double qtyInTier = Math.Min(remainingQty, tierRange);
                                totalAmount += (decimal)qtyInTier * tier.DonGia;
                                remainingQty -= qtyInTier;
                            }

                            hoaDon.AddDetail(dv.Id, dv.TenDichVu, cs.SoLuong, totalAmount / (decimal)cs.SoLuong, cs.ChiSoCu, cs.ChiSoMoi);
                        }
                        else
                        {
                            hoaDon.AddDetail(dv.Id, dv.TenDichVu, 1, bg.DonGia);
                        }
                    }

                    // Add fixed services (Management fee, etc.)
                    var bgQL = bangGias.FirstOrDefault(x => x.TenBangGia.Contains("quản lý"));
                    if (bgQL != null)
                    {
                        var dvQL = dichVus.First(x => x.Id == bgQL.DichVuId);
                        hoaDon.AddDetail(dvQL.Id, dvQL.TenDichVu, (double)canHo.DienTich, bgQL.DonGia);
                    }

                    // For odd months, mark as paid
                    if (month % 2 != 0)
                    {
                        hoaDon.AddThanhToan(hoaDon.CalculateTotalBalance(), DateTime.Now, PhuongThucThanhToan.ChuyenKhoan, $"PAY-{maHoaDon}", "Thanh toán hóa đơn");
                    }

                    hoaDons.Add(hoaDon);
                }
            }

            await context.HoaDons.AddRangeAsync(hoaDons);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} HoaDons.", hoaDons.Count);
        }
    }
}
