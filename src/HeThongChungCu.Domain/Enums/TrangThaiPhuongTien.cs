using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiPhuongTien : BaseEnum<TrangThaiPhuongTien, int>
{
    public static readonly TrangThaiPhuongTien Active = new(1, "Đang hoạt động");
    public static readonly TrangThaiPhuongTien Inactive = new(2, "Đã hủy");
    public static readonly TrangThaiPhuongTien Blocked = new(3, "Bị khóa");

    private TrangThaiPhuongTien(int value, string name) : base(value, name)
    {
    }
}
