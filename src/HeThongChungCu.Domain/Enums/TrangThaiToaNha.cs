using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiToaNha : BaseEnum<TrangThaiToaNha, int>
{
    public static readonly TrangThaiToaNha DangHoatDong = new(1, "Đang hoạt động");
    public static readonly TrangThaiToaNha BaoTri = new(2, "Bảo trì");
    public static readonly TrangThaiToaNha NgungHoatDong = new(3, "Ngưng hoạt động");

    private TrangThaiToaNha(int value, string name) : base(value, name)
    {
    }
}
