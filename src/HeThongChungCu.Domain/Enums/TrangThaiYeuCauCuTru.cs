using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiYeuCauCuTru : BaseEnum<TrangThaiYeuCauCuTru, int>
{
    public static readonly TrangThaiYeuCauCuTru Pending = new(1, "Đang chờ duyệt");
    public static readonly TrangThaiYeuCauCuTru Approved = new(2, "Đã duyệt");
    public static readonly TrangThaiYeuCauCuTru Rejected = new(3, "Từ chối");

    private TrangThaiYeuCauCuTru(int value, string name) : base(value, name)
    {
    }
}
