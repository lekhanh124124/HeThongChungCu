using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiYeuCau : BaseEnum<LoaiYeuCau, int>
{
    public static readonly LoaiYeuCau Them = new(1, "Thêm");
    public static readonly LoaiYeuCau Xoa = new(2, "Xóa");
    public static readonly LoaiYeuCau Sua = new(3, "Sửa");

    private LoaiYeuCau(int value, string name) : base(value, name)
    {
    }
}
