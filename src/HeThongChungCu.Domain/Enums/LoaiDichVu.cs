using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDichVu : BaseEnum<LoaiDichVu, int>
{
    public static readonly LoaiDichVu Dien = new(1, "Điện");
    public static readonly LoaiDichVu Nuoc = new(2, "Nước");
    public static readonly LoaiDichVu QuanLy = new(3, "Quản lý");
    public static readonly LoaiDichVu PhuongTien = new(4, "Phương tiện");
    public static readonly LoaiDichVu TienIch = new(5, "Tiện ích");
    public static readonly LoaiDichVu Khac = new(6, "Khác");

    private LoaiDichVu(int value, string name) : base(value, name)
    {
    }
}
