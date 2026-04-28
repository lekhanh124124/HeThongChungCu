using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiDotThanhToan : BaseEnum<TrangThaiDotThanhToan, int>
{
    public static readonly TrangThaiDotThanhToan Nhap = new(1, "Nháp");
    public static readonly TrangThaiDotThanhToan DaPhatHanh = new(2, "Đã phát hành");
    public static readonly TrangThaiDotThanhToan DaDong = new(3, "Đã đóng");

    private TrangThaiDotThanhToan(int value, string name) : base(value, name)
    {
    }
}
