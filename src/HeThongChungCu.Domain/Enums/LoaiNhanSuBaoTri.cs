using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiNhanSuBaoTri : BaseEnum<LoaiNhanSuBaoTri, int>
{
    public static readonly LoaiNhanSuBaoTri NoiBo = new(1, "Nội bộ");
    public static readonly LoaiNhanSuBaoTri DoiTac = new(2, "Đối tác");

    private LoaiNhanSuBaoTri(int value, string name) : base(value, name)
    {
    }
}
