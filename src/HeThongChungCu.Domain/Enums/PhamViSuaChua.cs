using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class PhamViSuaChua : BaseEnum<PhamViSuaChua, int>
{
    public static readonly PhamViSuaChua TrongCanHo = new(1, "Trong căn hộ");
    public static readonly PhamViSuaChua KhuVucChung = new(2, "Khu vực chung");

    private PhamViSuaChua(int value, string name) : base(value, name)
    {
    }
}
