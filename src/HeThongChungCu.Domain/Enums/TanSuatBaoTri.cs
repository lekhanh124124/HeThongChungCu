using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TanSuatBaoTri : BaseEnum<TanSuatBaoTri, int>
{
    public static readonly TanSuatBaoTri HangNgay = new(1, "Hàng ngày");
    public static readonly TanSuatBaoTri HangTuan = new(2, "Hàng tuần");
    public static readonly TanSuatBaoTri HangThang = new(3, "Hàng tháng");
    public static readonly TanSuatBaoTri HangQuy = new(4, "Hàng quý");
    public static readonly TanSuatBaoTri SauThang = new(5, "Sáu tháng");
    public static readonly TanSuatBaoTri HangNam = new(6, "Hàng năm");

    private TanSuatBaoTri(int value, string name) : base(value, name)
    {
    }
}
