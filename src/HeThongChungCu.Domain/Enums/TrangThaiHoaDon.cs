using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiHoaDon : BaseEnum<TrangThaiHoaDon, int>
{
    public static readonly TrangThaiHoaDon ChuaThanhToan = new(1, "Chưa thanh toán");
    public static readonly TrangThaiHoaDon DaThanhToan = new(2, "Đã thanh toán");
    public static readonly TrangThaiHoaDon ThanhToanMotPhan = new(3, "Thanh toán một phần");
    public static readonly TrangThaiHoaDon QuaHan = new(4, "Quá hạn");
    public static readonly TrangThaiHoaDon QuaHanNhe = new(5, "Quá hạn nhẹ");
    public static readonly TrangThaiHoaDon QuaHanNang = new(6, "Quá hạn nặng");
    public static readonly TrangThaiHoaDon DaHuy = new(7, "Đã hủy");

    private TrangThaiHoaDon(int value, string name) : base(value, name)
    {
    }
}
