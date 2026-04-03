using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Interfaces;

public interface IBillingService
{
    /// <summary>
    /// Tính toán số tiền dựa trên bảng giá và số lượng tiêu thụ.
    /// </summary>
    decimal CalculateAmount(BangGia priceList, decimal quantity);

    /// <summary>
    /// Tính tổng phí gửi xe dựa trên danh sách phương tiện và các dịch vụ tương ứng.
    /// </summary>
    decimal CalculateParkingFee(IEnumerable<PhuongTien> activeVehicles, IEnumerable<DichVu> parkingServices, DateTime calculationDate);
}
