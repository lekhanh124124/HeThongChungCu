using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class PhuongThucThanhToan : BaseEnum<PhuongThucThanhToan, int>
{
    public static readonly PhuongThucThanhToan TienMat = new(1, "Tiền mặt");
    public static readonly PhuongThucThanhToan ChuyenKhoan = new(2, "Chuyển khoản");
    public static readonly PhuongThucThanhToan ViDienTu = new(3, "Ví điện tử");

    private PhuongThucThanhToan(int value, string name) : base(value, name)
    {
    }
}
