using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiYeuCau : BaseEnum<TrangThaiYeuCau, int>
{
    public static readonly TrangThaiYeuCau Pending = new(1, "Đang chờ duyệt");
    public static readonly TrangThaiYeuCau Approved = new(2, "Đã duyệt");
    public static readonly TrangThaiYeuCau Rejected = new(3, "Từ chối");

    private TrangThaiYeuCau(int value, string name) : base(value, name)
    {
    }
}
