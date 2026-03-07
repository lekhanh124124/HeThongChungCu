using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class GioiTinh : BaseEnum<GioiTinh, int>
{
    public static readonly GioiTinh Nam = new(1, nameof(Nam));
    public static readonly GioiTinh Nu = new(2, nameof(Nu));
    public static readonly GioiTinh Khac = new(3, nameof(Khac));

    private GioiTinh(int value, string name) : base(value, name)
    {
    }
}
