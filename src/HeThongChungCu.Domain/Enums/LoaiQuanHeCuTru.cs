using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiQuanHeCuTru : BaseEnum<LoaiQuanHeCuTru, int>
{
    public static readonly LoaiQuanHeCuTru ChuHo = new(1, "Chủ hộ");
    public static readonly LoaiQuanHeCuTru VoChong = new(2, "Vợ/Chồng");
    public static readonly LoaiQuanHeCuTru ConCai = new(3, "Con cái");
    public static readonly LoaiQuanHeCuTru Khac = new(4, "Khác");

    private LoaiQuanHeCuTru(int value, string name) : base(value, name)
    {
    }
}
