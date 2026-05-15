using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiDichVu : BaseEnum<LoaiDichVu, int>
{
    public static readonly LoaiDichVu VanHanh = new(1, "Vận hành");
    public static readonly LoaiDichVu YeuCau = new(2, "Yêu cầu");
    public static readonly LoaiDichVu TienIch = new(3, "Tiện ích");
    public static readonly LoaiDichVu ThueNha = new(4, "Thuê nhà");
    public static readonly LoaiDichVu PhatTreHan = new(5, "Phạt trễ hạn");
    public static readonly LoaiDichVu Khac = new(6, "Khác");
    public static readonly LoaiDichVu YeuCauSuaChua = new(7, "Yêu cầu sửa chữa");
    public static readonly LoaiDichVu YeuCauThiCong = new(8, "Yêu cầu thi công");

    private LoaiDichVu(int value, string name) : base(value, name)
    {
    }
}
