using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiCuTru : BaseEnum<TrangThaiCuTru, int>
{
    public static readonly TrangThaiCuTru DangCuTru = new(1, "Đang cư trú");
    public static readonly TrangThaiCuTru DaKetThuc = new(2, "Đã kết thúc");
    public static readonly TrangThaiCuTru ChoDuyet = new(3, "Chờ duyệt");

    private TrangThaiCuTru(int value, string name) : base(value, name)
    {
    }
}
