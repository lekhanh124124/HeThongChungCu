using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiHanhDongYeuCau : BaseEnum<LoaiHanhDongYeuCau, int>
{
    public static readonly LoaiHanhDongYeuCau Them = new(1, "Thêm");
    public static readonly LoaiHanhDongYeuCau Sua = new(2, "Sửa");
    public static readonly LoaiHanhDongYeuCau Xoa = new(3, "Xóa");

    private LoaiHanhDongYeuCau(int value, string name) : base(value, name)
    {
    }
}
