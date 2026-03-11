using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiTang : BaseEnum<LoaiTang, int>
{
    public static readonly LoaiTang TangLau = new(1, "Tầng lầu");
    public static readonly LoaiTang TangHam = new(2, "Tầng hầm");

    private LoaiTang(int value, string name) : base(value, name)
    {
    }
}
