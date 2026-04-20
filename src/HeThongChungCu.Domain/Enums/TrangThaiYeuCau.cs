using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiYeuCau : BaseEnum<TrangThaiYeuCau, int>
{
    public static readonly TrangThaiYeuCau Pending = new(1, "Đang chờ duyệt");
    public static readonly TrangThaiYeuCau Approved = new(2, "Đã duyệt");
    public static readonly TrangThaiYeuCau Rejected = new(3, "Từ chối");
    public static readonly TrangThaiYeuCau Saved = new(4, "Đã lưu");
    public static readonly TrangThaiYeuCau Withdrawn = new(5, "Đã thu hồi");
    public static readonly TrangThaiYeuCau Invalidated = new(6, "Hết hiệu lực");
    public static readonly TrangThaiYeuCau Completed = new(7, "Hoàn tất");
    public static readonly TrangThaiYeuCau Cancelled = new(8, "Đã hủy");

    private TrangThaiYeuCau(int value, string name) : base(value, name)
    {
    }
}
