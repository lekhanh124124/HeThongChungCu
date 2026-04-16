using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDichVu : BaseEnum<LoaiDichVu, int>
{
    public static readonly LoaiDichVu VanHanh = new(1, "Vận hành");
    public static readonly LoaiDichVu SuaChua = new(2, "Sửa chữa");
    public static readonly LoaiDichVu TienIch = new(3, "Tiện ích");
    public static readonly LoaiDichVu Khac = new(4, "Khác");

    private LoaiDichVu(int value, string name) : base(value, name)
    {
    }
}
