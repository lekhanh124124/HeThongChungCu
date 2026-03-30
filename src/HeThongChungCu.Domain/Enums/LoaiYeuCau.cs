using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiYeuCau : BaseEnum<LoaiYeuCau, int>
{
    public static readonly LoaiYeuCau Them = new(1, "Thêm");
    public static readonly LoaiYeuCau Sua = new(2, "Sửa");
    public static readonly LoaiYeuCau Xoa = new(3, "Xóa");

    private LoaiYeuCau(int value, string name) : base(value, name)
    {
    }
}
