using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiHoaDon : BaseEnum<TrangThaiHoaDon, int>
{
    public static readonly TrangThaiHoaDon ChoDuyet = new(1, "Chờ duyệt");
    public static readonly TrangThaiHoaDon ChuaThanhToan = new(2, "Chưa thanh toán");
    public static readonly TrangThaiHoaDon DaThanhToan = new(3, "Đã thanh toán");
    public static readonly TrangThaiHoaDon ThanhToanMotPhan = new(4, "Thanh toán một phần");
    public static readonly TrangThaiHoaDon QuaHan = new(5, "Quá hạn");
    public static readonly TrangThaiHoaDon QuaHanNhe = new(6, "Quá hạn nhẹ");
    public static readonly TrangThaiHoaDon QuaHanNang = new(7, "Quá hạn nặng");
    public static readonly TrangThaiHoaDon DaHuy = new(8, "Đã hủy");

    private TrangThaiHoaDon(int value, string name) : base(value, name)
    {
    }
}
