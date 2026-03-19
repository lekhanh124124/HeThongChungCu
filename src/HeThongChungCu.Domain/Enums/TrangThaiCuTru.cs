using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiCuTru : BaseEnum<TrangThaiCuTru, int>
{
    public static readonly TrangThaiCuTru DangCuTru = new(1, "Đang cư trú");
    public static readonly TrangThaiCuTru DaKetThuc = new(2, "Đã kết thúc");

    private TrangThaiCuTru(int value, string name) : base(value, name)
    {
    }
}
