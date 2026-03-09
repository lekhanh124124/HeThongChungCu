using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDichVu : BaseEnum<LoaiDichVu, int>
{
    public static readonly LoaiDichVu Dien = new(1, "Điện");
    public static readonly LoaiDichVu Nuoc = new(2, "Nước");
    public static readonly LoaiDichVu Internet = new(3, "Internet");
    public static readonly LoaiDichVu Rac = new(4, "Rác");

    private LoaiDichVu(int value, string name) : base(value, name)
    {
    }
}
