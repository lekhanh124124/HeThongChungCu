using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiDangKy : BaseEnum<TrangThaiDangKy, int>
{
    public static readonly TrangThaiDangKy ChoDuyet = new(1, "Chờ duyệt");
    public static readonly TrangThaiDangKy DangSuDung = new(2, "Đang sử dụng");
    public static readonly TrangThaiDangKy TamNgung = new(3, "Tạm ngưng");
    public static readonly TrangThaiDangKy DaHuy = new(4, "Đã hủy");

    private TrangThaiDangKy(int value, string name) : base(value, name)
    {
    }
}
