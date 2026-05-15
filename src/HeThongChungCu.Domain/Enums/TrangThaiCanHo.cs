using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiCanHo : BaseEnum<TrangThaiCanHo, int>
{
    public static readonly TrangThaiCanHo ChuaBanGiao = new(1, "Chưa bàn giao");
    public static readonly TrangThaiCanHo DangTrong = new(2, "Đang trống");
    public static readonly TrangThaiCanHo DangChoThue = new(3, "Đang cho thuê");
    public static readonly TrangThaiCanHo DaBanGiao = new(4, "Đã bàn giao");
    public static readonly TrangThaiCanHo DangThiCong = new(5, "Đang thi công");

    private TrangThaiCanHo(int value, string name) : base(value, name)
    {
    }
}
