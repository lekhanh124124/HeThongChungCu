using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThanhToan : BaseEnum<TrangThaiThanhToan, int>
{
    public static readonly TrangThaiThanhToan ChoThanhToan = new(1, "Chờ thanh toán");
    public static readonly TrangThaiThanhToan ThanhCong = new(2, "Thành công");
    public static readonly TrangThaiThanhToan ThatBai = new(3, "Thất bại");
    public static readonly TrangThaiThanhToan HetHan = new(4, "Hết hạn");

    private TrangThaiThanhToan(int value, string name) : base(value, name) { }
}
