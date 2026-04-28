using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public abstract class ChiTietHoaDon : AuditableEntity
{
    public int HoaDonId { get; private set; }
    public LoaiChiTietHoaDon LoaiChiTietHoaDonId { get; protected set; } = null!;
    public string TenMucPhi { get; private set; } = null!;
    public decimal SoLuong { get; private set; }
    public decimal DonGia { get; private set; }
    public decimal ThanhTien { get; private set; }
    public string? GhiChu { get; private set; }

    protected ChiTietHoaDon() { } // EF Core

    protected ChiTietHoaDon(
        int hoaDonId,
        LoaiChiTietHoaDon loaiChiTietHoaDonId,
        string tenMucPhi,
        decimal soLuong,
        decimal donGia,
        string? ghiChu)
    {
        HoaDonId = hoaDonId;
        LoaiChiTietHoaDonId = loaiChiTietHoaDonId;
        TenMucPhi = tenMucPhi;
        SoLuong = soLuong;
        DonGia = donGia;
        GhiChu = ghiChu;
        ThanhTien = soLuong * donGia;
    }
}
