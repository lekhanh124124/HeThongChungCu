using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietHoaDonThiCong : ChiTietHoaDon
{
    public int YeuCauThiCongId { get; private set; }
    public LoaiChiPhiThiCong LoaiChiPhiThiCongId { get; private set; } = null!;

    private ChiTietHoaDonThiCong() : base() { }

    internal ChiTietHoaDonThiCong(
        int hoaDonId,
        int yeuCauThiCongId,
        LoaiChiPhiThiCong loaiChiPhiThiCongId,
        string tenMucPhi,
        decimal soTien,
        string? ghiChu = null)
        : base(hoaDonId, LoaiChiTietHoaDon.ThiCong, tenMucPhi, 1, soTien, ghiChu)
    {
        YeuCauThiCongId = yeuCauThiCongId;
        LoaiChiPhiThiCongId = loaiChiPhiThiCongId;
    }
}
