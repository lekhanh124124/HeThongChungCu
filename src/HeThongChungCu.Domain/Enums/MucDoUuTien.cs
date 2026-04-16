using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class MucDoUuTien : BaseEnum<MucDoUuTien, int>
{
    public static readonly MucDoUuTien Thap = new(1, "Thấp");
    public static readonly MucDoUuTien Thuong = new(2, "Thường");
    public static readonly MucDoUuTien Cao = new(3, "Cao");
    public static readonly MucDoUuTien KhanCap = new(4, "Khẩn cấp");

    private MucDoUuTien(int value, string name) : base(value, name)
    {
    }
}
