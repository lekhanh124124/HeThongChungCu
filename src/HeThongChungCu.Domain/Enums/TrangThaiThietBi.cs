using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThietBi : BaseEnum<TrangThaiThietBi, int>
{
    public static readonly TrangThaiThietBi HoatDongTot = new(1, "Hoạt động tốt");
    public static readonly TrangThaiThietBi CanBaoTri = new(2, "Cần bảo trì");
    public static readonly TrangThaiThietBi DangBaoTri = new(3, "Đang bảo trì");
    public static readonly TrangThaiThietBi DangHong = new(4, "Đang hỏng");
    public static readonly TrangThaiThietBi NgungSuDung = new(5, "Ngừng sử dụng");

    private TrangThaiThietBi(int value, string name) : base(value, name)
    {
    }
}
