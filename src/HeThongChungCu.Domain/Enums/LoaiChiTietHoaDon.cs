using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiChiTietHoaDon : BaseEnum<LoaiChiTietHoaDon, int>
{
    public static readonly LoaiChiTietHoaDon DichVu = new(1, "Dịch vụ");
    public static readonly LoaiChiTietHoaDon LaiChamTra = new(2, "Lãi chậm trả");
    public static readonly LoaiChiTietHoaDon NoCu = new(3, "Nợ cũ");

    private LoaiChiTietHoaDon(int value, string name) : base(value, name)
    {
    }
}
