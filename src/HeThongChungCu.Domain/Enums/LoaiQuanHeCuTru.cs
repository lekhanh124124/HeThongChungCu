using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiQuanHeCuTru : BaseEnum<LoaiQuanHeCuTru, int>
{
    public static readonly LoaiQuanHeCuTru ChuHo = new(1, nameof(ChuHo));
    public static readonly LoaiQuanHeCuTru VoChong = new(2, nameof(VoChong));
    public static readonly LoaiQuanHeCuTru ConCai = new(3, nameof(ConCai));
    public static readonly LoaiQuanHeCuTru Khac = new(4, nameof(Khac));

    private LoaiQuanHeCuTru(int value, string name) : base(value, name)
    {
    }
}
