using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class BillingService : IBillingService
{
    public decimal CalculateAmount(BangGia priceList, decimal quantity)
    {
        // 1. Nếu là giá lũy tiến (Điện, Nước...)
        if (priceList.LoaiDinhGiaId == LoaiDinhGia.LuyTien)
        {
            if (priceList.BangGiaLuyTiens.Count == 0)
                throw new BusinessException("Bảng giá lũy tiến chưa có các bậc giá.");

            decimal totalAmount = 0;
            var sortedTiers = priceList.BangGiaLuyTiens.OrderBy(t => t.TuMuc).ToList();

            foreach (var tier in sortedTiers)
            {
                if (quantity <= tier.TuMuc) break;

                decimal consumptionInTier;
                if (tier.DenMuc == null || quantity <= tier.DenMuc)
                {
                    consumptionInTier = quantity - tier.TuMuc;
                }
                else
                {
                    consumptionInTier = tier.DenMuc.Value - tier.TuMuc;
                }

                totalAmount += consumptionInTier * tier.DonGia;

                if (tier.DenMuc == null || quantity <= tier.DenMuc) break;
            }

            return totalAmount;
        }

        // 2. Giá cố định
        return Math.Round(quantity * priceList.DonGia, 0);
    }

    public decimal CalculateParkingFee(IEnumerable<PhuongTien> activeVehicles, IEnumerable<DichVu> parkingServices, DateTime calculationDate)
    {
        decimal totalFee = 0;

        // 1. Nhóm phương tiện theo loại
        var vehiclesByType = activeVehicles
            .Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active)
            .GroupBy(v => v.LoaiPhuongTienId);

        foreach (var group in vehiclesByType)
        {
            var loaiPhuongTien = group.Key;
            var vehicleCount = group.Count();

            // 2. Tìm dịch vụ tương ứng
            var service = parkingServices.FirstOrDefault(s => s.MaDichVu == loaiPhuongTien.DefaultServiceCode);

            if (service == null) continue;

            // 3. Lấy bảng giá đang áp dụng
            var currentPrice = service.GetCurrentPrice(calculationDate);
            if (currentPrice != null)
            {
                // 4. Gọi logic tính tiền chung
                totalFee += CalculateAmount(currentPrice, vehicleCount);
            }
        }

        return totalFee;
    }
}
