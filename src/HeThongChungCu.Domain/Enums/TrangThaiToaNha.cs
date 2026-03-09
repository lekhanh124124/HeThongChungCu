using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiToaNha : BaseEnum<TrangThaiToaNha, int>
{
    public static readonly TrangThaiToaNha HoatDong = new(1, "Hoạt động");
    public static readonly TrangThaiToaNha BaoTri = new(2, "Bảo trì");
    public static readonly TrangThaiToaNha NgungHoatDong = new(3, "Ngưng hoạt động");

    private TrangThaiToaNha(int value, string name) : base(value, name)
    {
    }
}
