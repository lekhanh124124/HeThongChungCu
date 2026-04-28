using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietHoaDonDichVu : ChiTietHoaDon
{
    private ChiTietHoaDonDichVu() : base() { }

    public int DichVuId { get; private set; }

    internal ChiTietHoaDonDichVu(
        int hoaDonId,
        string tenMucPhi,
        decimal soLuong,
        decimal donGia,
        int dichVuId,
        string? ghiChu = null)
        : base(hoaDonId, LoaiChiTietHoaDon.DichVu, tenMucPhi, soLuong, donGia, ghiChu)
    {
        DichVuId = dichVuId;
    }
}
