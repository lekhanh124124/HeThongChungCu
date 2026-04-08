using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietGiaKhungGio : AuditableEntity
{
    public int BangGiaId { get; private set; }
    public int KhungGioId { get; private set; }
    public GiaTien DonGia { get; private set; } = null!;

    // Navigation properties
    public BangGiaKhungGio BangGia { get; private set; } = null!;
    public KhungGioDichVu KhungGio { get; private set; } = null!;

    private ChiTietGiaKhungGio() { } // EF Core

    public ChiTietGiaKhungGio(int bangGiaId, int khungGioId, decimal donGia)
    {
        BangGiaId = bangGiaId;
        KhungGioId = khungGioId;
        DonGia = new GiaTien(donGia);
    }

    public void UpdateDonGia(decimal donGia)
    {
        DonGia = new GiaTien(donGia);
    }
}
