using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThiCong : BaseEnum<TrangThaiThiCong, int>
{
    public static readonly TrangThaiThiCong ChuaThiCong = new(1, "Chưa thi công");
    public static readonly TrangThaiThiCong ChoThuCoc = new(2, "Chờ thu tiền cọc");
    public static readonly TrangThaiThiCong DaCapPhep = new(3, "Đã cấp phép");
    public static readonly TrangThaiThiCong DaHoanTat = new(4, "Đã hoàn tất");

    private TrangThaiThiCong(int value, string name) : base(value, name)
    {
    }
}
