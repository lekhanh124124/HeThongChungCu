using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TinhTrangCanHo : BaseEnum<TinhTrangCanHo, int>
{
    public static readonly TinhTrangCanHo Trong = new(1, "Trống");
    public static readonly TinhTrangCanHo DaThue = new(2, "Đã thuê");
    public static readonly TinhTrangCanHo DaBan = new(3, "Đã bán");

    private TinhTrangCanHo(int value, string name) : base(value, name)
    {
    }
}
