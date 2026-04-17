using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiTaiLieu : BaseEnum<LoaiTaiLieu, int>
{
    public static readonly LoaiTaiLieu NguoiDung = new(1, nameof(NguoiDung));
    public static readonly LoaiTaiLieu YeuCauCuTru = new(2, nameof(YeuCauCuTru));

    private LoaiTaiLieu(int value, string name) : base(value, name)
    {
    }
}
