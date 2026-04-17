using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiNhanSuYeuCau : BaseEnum<LoaiNhanSuYeuCau, int>
{
    public static readonly LoaiNhanSuYeuCau SuaChua = new(1, "Sửa chữa");
    public static readonly LoaiNhanSuYeuCau ThiCong = new(2, "Thi công");

    private LoaiNhanSuYeuCau(int value, string name) : base(value, name)
    {
    }
}
