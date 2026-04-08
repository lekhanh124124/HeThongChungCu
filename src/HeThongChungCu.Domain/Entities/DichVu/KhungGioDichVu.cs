using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class KhungGioDichVu : AuditableEntity
{
    public int DichVuId { get; private set; }
    public TimeSpan GioBatDau { get; private set; }
    public TimeSpan GioKetThuc { get; private set; }
    public string TenKhungGio { get; private set; } = string.Empty;
    public NgayTrongTuan? NgayTrongTuan { get; private set; } // Null = Mọi ngày.
    public bool IsActive { get; private set; }

    // Navigation property
    public DichVu DichVu { get; private set; } = null!;

    private KhungGioDichVu() { } // EF Core

    public KhungGioDichVu(
        int dichVuId,
        TimeSpan gioBatDau,
        TimeSpan gioKetThuc,
        string tenKhungGio,
        NgayTrongTuan? ngayTrongTuan = null)
    {
        if (gioBatDau >= gioKetThuc)
            throw new BusinessException("Giờ bắt đầu phải trước giờ kết thúc.");
        
        DichVuId = dichVuId;
        GioBatDau = gioBatDau;
        GioKetThuc = gioKetThuc;
        TenKhungGio = tenKhungGio;
        NgayTrongTuan = ngayTrongTuan;
        IsActive = false;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public bool OverlapsWith(TimeSpan start, TimeSpan end, NgayTrongTuan? dayOfWeek)
    {
        if (!IsActive) return false;

        // Check day overlap:
        // If either is null (all days), there's a potential day overlap.
        // If both have values, they must be the same value to overlap.
        bool dayOverlaps = NgayTrongTuan is null || dayOfWeek is null || NgayTrongTuan == dayOfWeek;

        if (!dayOverlaps) return false;

        // Check time overlap: (StartA < EndB) && (EndA > StartB)
        return GioBatDau < end && GioKetThuc > start;
    }
}
