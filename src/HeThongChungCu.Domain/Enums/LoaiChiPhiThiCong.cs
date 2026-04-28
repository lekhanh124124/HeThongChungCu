using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiChiPhiThiCong : BaseEnum<LoaiChiPhiThiCong, int>
{
    public static readonly LoaiChiPhiThiCong DatCoc = new(1, "Đặt cọc thi công");
    public static readonly LoaiChiPhiThiCong PhatViPham = new(2, "Phạt vi phạm thi công");

    private LoaiChiPhiThiCong(int value, string name) : base(value, name)
    {
    }
}
