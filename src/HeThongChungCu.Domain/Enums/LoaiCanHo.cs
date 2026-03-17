using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiCanHo : BaseEnum<LoaiCanHo, int>
{
    public static readonly LoaiCanHo Standard = new(1, "Standard");
    public static readonly LoaiCanHo Studio = new(2, "Studio");
    public static readonly LoaiCanHo Penthouse = new(3, "Penthouse");
    public static readonly LoaiCanHo Shophouse = new(4, "Shophouse");

    private LoaiCanHo(int value, string name) : base(value, name)
    {
    }
}
