using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ThanhToan : BaseEntity
{
    public int HoaDonId { get; private set; }
    public DateTime NgayThanhToan { get; private set; }
    public decimal SoTien { get; private set; }
    public PhuongThucThanhToan PhuongThucThanhToanId { get; private set; } = null!;
    public string MaGiaoDich { get; private set; } = string.Empty;
    public string NoiDung { get; private set; } = string.Empty;

    private ThanhToan() { } // EF Core

    internal ThanhToan(
        int hoaDonId,
        DateTime ngayThanhToan,
        decimal soTien,
        PhuongThucThanhToan phuongThucThanhToanId,
        string maGiaoDich = "",
        string noiDung = "")
    {
        HoaDonId = hoaDonId;
        NgayThanhToan = ngayThanhToan;
        SoTien = soTien;
        PhuongThucThanhToanId = phuongThucThanhToanId;
        MaGiaoDich = maGiaoDich;
        NoiDung = noiDung;
    }
}
