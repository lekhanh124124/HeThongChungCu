using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiSuCoKyThuat : BaseEnum<LoaiSuCoKyThuat, int>
{
    public static readonly LoaiSuCoKyThuat Dien = new(1, "Điện");
    public static readonly LoaiSuCoKyThuat Nuoc = new(2, "Nước");
    public static readonly LoaiSuCoKyThuat KhoaCua = new(3, "Khóa/Cửa");
    public static readonly LoaiSuCoKyThuat DieuHoa = new(4, "Điều hòa");
    public static readonly LoaiSuCoKyThuat ThangMay = new(5, "Thang máy");
    public static readonly LoaiSuCoKyThuat ChieuSang = new(6, "Chiếu sáng");
    public static readonly LoaiSuCoKyThuat Khac = new(99, "Khác");

    private LoaiSuCoKyThuat(int value, string name) : base(value, name)
    {
    }
}
