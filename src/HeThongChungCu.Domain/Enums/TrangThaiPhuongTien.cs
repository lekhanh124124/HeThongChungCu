using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiPhuongTien : BaseEnum<TrangThaiPhuongTien, int>
{
    public static readonly TrangThaiPhuongTien PendingApproval = new(1, "Chờ duyệt");
    public static readonly TrangThaiPhuongTien Approved = new(2, "Đã duyệt");
    public static readonly TrangThaiPhuongTien Rejected = new(3, "Từ chối");

    private TrangThaiPhuongTien(int value, string name) : base(value, name)
    {
    }
}
