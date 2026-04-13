using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class HoaDonDoiTac : AggregateRoot
{
    public int HopDongDoiTacId { get; private set; }
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public GiaTien SoTien { get; private set; } = null!;
    public DateTimeOffset NgayGhiNhan { get; private set; }
    public string? GhiChu { get; private set; }
    public TrangThaiThanhToanDoiTac TrangThaiThanhToanId { get; private set; } = null!;

    public int? FileHoaDonId { get; private set; }
    public virtual TepTaiLieu? FileHoaDon { get; private set; }

    private HoaDonDoiTac() { } // EF Core

    public HoaDonDoiTac(
        int hopDongDoiTacId,
        int thang,
        int nam,
        decimal soTien,
        int? fileHoaDonId = null,
        string? ghiChu = null)
    {
        if (thang < 1 || thang > 12)
            throw new BusinessException("Tháng không hợp lệ.");

        HopDongDoiTacId = hopDongDoiTacId;
        Thang = thang;
        Nam = nam;
        SoTien = new GiaTien(soTien);
        NgayGhiNhan = DateTimeOffset.Now;
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
        if (thang < 1 || thang > 12)
            throw new BusinessException("Tháng không hợp lệ.");

        Thang = thang;
        Nam = nam;
        SoTien = new GiaTien(soTien);
        FileHoaDonId = fileHoaDonId;
        GhiChu = ghiChu;
    }

    public void UpdateStatus(TrangThaiThanhToanDoiTac nextStatus)
    {
        TrangThaiThanhToanId = nextStatus;
    }
}
