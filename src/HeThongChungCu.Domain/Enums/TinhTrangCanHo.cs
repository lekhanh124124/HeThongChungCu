using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TinhTrangCanHo : BaseEnum<TinhTrangCanHo, int>
{
    public static readonly TinhTrangCanHo Trong = new(1, nameof(Trong));
    public static readonly TinhTrangCanHo DaThue = new(2, nameof(DaThue));
    public static readonly TinhTrangCanHo DaBan = new(3, nameof(DaBan));

    private TinhTrangCanHo(int value, string name) : base(value, name)
    {
    }
}
