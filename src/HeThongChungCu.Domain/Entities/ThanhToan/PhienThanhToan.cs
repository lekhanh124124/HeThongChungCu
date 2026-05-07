using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class PhienThanhToan : AuditableEntity
{
    public string MaThanhToan { get; private set; } = null!;
    public int HoaDonId { get; private set; }
    public string ChiTietHoaDonIds { get; private set; } = null!; // Dạng JSON hoặc CSV
    public decimal SoTien { get; private set; }
    public int TrangThaiThanhToanId { get; private set; }
    public string? GhiChu { get; private set; }

    private PhienThanhToan() { } // EF Core

    public PhienThanhToan(
        string maThanhToan,
        int hoaDonId,
        string chiTietHoaDonIds,
        decimal soTien,
        string? ghiChu = null)
    {
        MaThanhToan = maThanhToan;
        HoaDonId = hoaDonId;
        ChiTietHoaDonIds = chiTietHoaDonIds;
        SoTien = soTien;
        TrangThaiThanhToanId = TrangThaiThanhToan.ChoThanhToan.Value;
        GhiChu = ghiChu;
    }

    public void UpdateStatus(TrangThaiThanhToan status)
    {
        TrangThaiThanhToanId = status.Value;
    }
}
