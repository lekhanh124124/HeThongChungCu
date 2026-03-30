using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiThePhuongTien : BaseEnum<TrangThaiThePhuongTien, int>
{
    public static readonly TrangThaiThePhuongTien Active = new(1, "Đang hoạt động");
    public static readonly TrangThaiThePhuongTien Locked = new(2, "Bị khóa");
    public static readonly TrangThaiThePhuongTien Lost = new(3, "Báo mất");

    private TrangThaiThePhuongTien(int value, string name) : base(value, name)
    {
    }
}
