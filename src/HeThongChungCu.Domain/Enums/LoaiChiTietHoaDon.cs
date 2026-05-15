using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class LoaiChiTietHoaDon : BaseEnum<LoaiChiTietHoaDon, int>
{
    public static readonly LoaiChiTietHoaDon DichVu = new(1, "Dịch vụ");
    public static readonly LoaiChiTietHoaDon TieuThu = new(2, "Tiêu thụ");
    public static readonly LoaiChiTietHoaDon SuaChua = new(3, "Sửa chữa", LoaiDichVu.YeuCauSuaChua);
    public static readonly LoaiChiTietHoaDon ThiCong = new(4, "Thi công", LoaiDichVu.YeuCauThiCong);

    public LoaiDichVu? TuongUngLoaiDichVu { get; }

    private LoaiChiTietHoaDon(int value, string name, LoaiDichVu? tuongUngLoaiDichVu = null) : base(value, name)
    {
        TuongUngLoaiDichVu = tuongUngLoaiDichVu;
    }
}
