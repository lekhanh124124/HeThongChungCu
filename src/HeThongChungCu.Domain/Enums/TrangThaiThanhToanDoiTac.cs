using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThanhToanDoiTac : BaseEnum<TrangThaiThanhToanDoiTac, int>
{
    public static readonly TrangThaiThanhToanDoiTac ChuaThanhToan = new(1, "Chưa thanh toán");
    public static readonly TrangThaiThanhToanDoiTac DaThanhToan = new(2, "Đã thanh toán");

    private TrangThaiThanhToanDoiTac(int value, string name) : base(value, name)
    {
    }
}
