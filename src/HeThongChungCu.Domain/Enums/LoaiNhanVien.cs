using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiNhanVien : BaseEnum<LoaiNhanVien, int>
{
    public static readonly LoaiNhanVien KyThuat = new(1, "Kỹ thuật");
    public static readonly LoaiNhanVien VeSinh = new(2, "Vệ sinh");
    public static readonly LoaiNhanVien BaoVe = new(3, "Bảo vệ");
    public static readonly LoaiNhanVien QuanLy = new(4, "Quản lý");

    private LoaiNhanVien(int value, string name) : base(value, name)
    {
    }
}
