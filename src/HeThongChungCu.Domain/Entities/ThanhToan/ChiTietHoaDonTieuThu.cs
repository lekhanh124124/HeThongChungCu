using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietHoaDonTieuThu : ChiTietHoaDon
{
    public decimal ChiSoCu { get; private set; }
    public decimal ChiSoMoi { get; private set; }
    public int DichVuId { get; private set; }

    private ChiTietHoaDonTieuThu() : base() { }

    internal ChiTietHoaDonTieuThu(
        int hoaDonId,
        string tenMucPhi,
        decimal chiSoCu,
        decimal chiSoMoi,
        decimal donGia,
        int dichVuId,
        string? ghiChu = null)
        : base(hoaDonId, LoaiChiTietHoaDon.TieuThu, tenMucPhi, chiSoMoi - chiSoCu, donGia, ghiChu)
    {
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        DichVuId = dichVuId;
    }
}
