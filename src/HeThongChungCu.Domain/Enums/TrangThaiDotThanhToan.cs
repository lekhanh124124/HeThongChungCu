using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiDotThanhToan : BaseEnum<TrangThaiDotThanhToan, int>
{
    public static readonly TrangThaiDotThanhToan TaoMoi = new(1, "Tạo mới");
    public static readonly TrangThaiDotThanhToan DaPhatHanh = new(2, "Đã phát hành");
    public static readonly TrangThaiDotThanhToan DaDuyet = new(3, "Đã duyệt");

    private TrangThaiDotThanhToan(int value, string name) : base(value, name)
    {
    }
}
