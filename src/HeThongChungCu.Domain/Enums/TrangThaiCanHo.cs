using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiCanHo : BaseEnum<TrangThaiCanHo, int>
{
    public static readonly TrangThaiCanHo ChuaBanGiao = new(1, "Chưa bàn giao");
    public static readonly TrangThaiCanHo DangTrong = new(2, "Đang trống");
    public static readonly TrangThaiCanHo CoCuDan = new(3, "Có cư dân");
    public static readonly TrangThaiCanHo DangThiCong = new(4, "Đang thi công");

    private TrangThaiCanHo(int value, string name) : base(value, name)
    {
    }
}
