using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiGiayTo : BaseEnum<LoaiGiayTo, int>
{
    public static readonly LoaiGiayTo CCCD = new(1, "Căn cước công dân");
    public static readonly LoaiGiayTo SoHoKhau = new(2, "Sổ hộ khẩu");
    public static readonly LoaiGiayTo GiayKhaiSinh = new(3, "Giấy khai sinh");
    public static readonly LoaiGiayTo HopDongThue = new(4, "Hợp đồng thuê");
    public static readonly LoaiGiayTo Khac = new(5, "Khác");

    private LoaiGiayTo(int value, string name) : base(value, name)
    {
    }
}
