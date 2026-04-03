using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class HoaDonDoiTac : AggregateRoot
{
    public int DoiTacId { get; private set; }
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public decimal SoTien { get; private set; }
    public DateTime NgayGhiNhan { get; private set; }
    public int? FileHoaDonId { get; private set; }
    public string? GhiChu { get; private set; }
    public TrangThaiThanhToanDoiTac TrangThaiThanhToanId { get; private set; } = null!;

    private HoaDonDoiTac() { } // EF Core

    public HoaDonDoiTac(
        int doiTacId,
        int thang,
        int nam,
        decimal soTien,
        int? fileHoaDonId = null,
        string? ghiChu = null)
    {
        if (soTien < 0)
            throw new BusinessException("Số tiền hóa đơn không được nhỏ hơn 0.");
        
        if (thang < 1 || thang > 12)
            throw new BusinessException("Tháng không hợp lệ.");

        DoiTacId = doiTacId;
        Thang = thang;
        Nam = nam;
        SoTien = soTien;
        NgayGhiNhan = DateTime.Now;
        FileHoaDonId = fileHoaDonId;
        GhiChu = ghiChu;
        TrangThaiThanhToanId = TrangThaiThanhToanDoiTac.ChuaThanhToan;
    }

    public void UpdateInfo(
        int thang,
        int nam,
        decimal soTien,
        int? fileHoaDonId = null,
        string? ghiChu = null)
    {
        if (soTien < 0)
            throw new BusinessException("Số tiền hóa đơn không được nhỏ hơn 0.");
        
        if (thang < 1 || thang > 12)
            throw new BusinessException("Tháng không hợp lệ.");

        Thang = thang;
        Nam = nam;
        SoTien = soTien;
        FileHoaDonId = fileHoaDonId;
        GhiChu = ghiChu;
    }

    public void UpdateStatus(TrangThaiThanhToanDoiTac nextStatus)
    {
        TrangThaiThanhToanId = nextStatus;
    }
}
