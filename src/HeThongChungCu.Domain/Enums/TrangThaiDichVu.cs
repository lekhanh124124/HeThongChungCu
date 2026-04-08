using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiDichVu : BaseEnum<TrangThaiDichVu, int>
{
    public static readonly TrangThaiDichVu HoatDong = new(1, "Hoạt động");
    public static readonly TrangThaiDichVu CanhBao = new(2, "Cảnh báo (Hợp đồng hết hạn)");
    public static readonly TrangThaiDichVu NgungCungCap = new(3, "Ngưng cung cấp");
    public static readonly TrangThaiDichVu TaoMoi = new(4, "Tạo mới");

    private TrangThaiDichVu(int value, string name) : base(value, name)
    {
    }
}
