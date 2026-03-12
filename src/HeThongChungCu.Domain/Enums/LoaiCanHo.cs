using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiCanHo : BaseEnum<LoaiCanHo, int>
{
    public static readonly LoaiCanHo PN1 = new(1, "1 Phòng ngủ");
    public static readonly LoaiCanHo PN2 = new(2, "2 Phòng ngủ");
    public static readonly LoaiCanHo PN3 = new(3, "3 Phòng ngủ");
    public static readonly LoaiCanHo Studio = new(4, "Studio");
    public static readonly LoaiCanHo Penthouse = new(5, "Penthouse");
    public static readonly LoaiCanHo Shophouse = new(6, "Shophouse");

    private LoaiCanHo(int value, string name) : base(value, name)
    {
    }
}
