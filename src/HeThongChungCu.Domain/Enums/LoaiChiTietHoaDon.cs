using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiChiTietHoaDon : BaseEnum<LoaiChiTietHoaDon, int>
{
    public static readonly LoaiChiTietHoaDon DichVu = new(1, "Dịch vụ");
    public static readonly LoaiChiTietHoaDon TieuThu = new(2, "Tiêu thụ");
    public static readonly LoaiChiTietHoaDon SuaChua = new(3, "Sửa chữa");
    public static readonly LoaiChiTietHoaDon ThiCong = new(4, "Thi công");

    private LoaiChiTietHoaDon(int value, string name) : base(value, name)
    {
    }
}
