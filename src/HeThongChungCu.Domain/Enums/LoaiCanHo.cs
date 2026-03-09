using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiCanHo : BaseEnum<LoaiCanHo, int>
{
    public static readonly LoaiCanHo CanHo = new(1, "Căn hộ");
    public static readonly LoaiCanHo Studio = new(2, "Studio");
    public static readonly LoaiCanHo Penthouse = new(3, "Penthouse");
    public static readonly LoaiCanHo Shophouse = new(4, "Shophouse");

    private LoaiCanHo(int value, string name) : base(value, name)
    {
    }
}
