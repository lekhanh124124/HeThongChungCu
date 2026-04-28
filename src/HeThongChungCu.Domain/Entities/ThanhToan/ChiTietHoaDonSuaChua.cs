using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietHoaDonSuaChua : ChiTietHoaDon
{
    public int YeuCauSuaChuaId { get; private set; }

    private ChiTietHoaDonSuaChua() : base() { }

    internal ChiTietHoaDonSuaChua(
        int hoaDonId,
        int yeuCauSuaChuaId,
        string tenMucPhi,
        decimal soTien,
        string? ghiChu = null)
        : base(hoaDonId, LoaiChiTietHoaDon.SuaChua, tenMucPhi, 1, soTien, ghiChu)
    {
        YeuCauSuaChuaId = yeuCauSuaChuaId;
    }
}
