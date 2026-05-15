using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiThuChi : BaseEnum<LoaiThuChi, int>
{
    public static readonly LoaiThuChi Thu = new(1, "Thu");
    public static readonly LoaiThuChi Chi = new(2, "Chi");

    private LoaiThuChi(int value, string name) : base(value, name)
    {
    }
}
