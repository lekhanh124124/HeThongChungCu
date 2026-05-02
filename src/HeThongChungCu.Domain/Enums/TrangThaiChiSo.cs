using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class TrangThaiChiSo : BaseEnum<TrangThaiChiSo, int>
{
    public static readonly TrangThaiChiSo Draft = new(1, "Nháp");
    public static readonly TrangThaiChiSo Confirmed = new(2, "Đã xác nhận");
    public static readonly TrangThaiChiSo Locked = new(3, "Đã chốt/Đã lập hóa đơn");

    private TrangThaiChiSo(int value, string name) : base(value, name)
    {
    }
}
