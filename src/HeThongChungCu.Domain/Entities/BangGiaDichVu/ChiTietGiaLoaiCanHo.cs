using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietGiaLoaiCanHo : AuditableEntity
{
    public int BangGiaId { get; private set; }
    public LoaiCanHo? LoaiCanHoId { get; private set; }
    public GiaTien DonGia { get; private set; } = null!;

    // Navigation properties
    public BangGiaLoaiCanHo BangGia { get; private set; } = null!;

    private ChiTietGiaLoaiCanHo() { } // EF Core

    public ChiTietGiaLoaiCanHo(int bangGiaId, LoaiCanHo? loaiCanHoId, decimal donGia)
    {
        BangGiaId = bangGiaId;
        LoaiCanHoId = loaiCanHoId;
        DonGia = new GiaTien(donGia);
    }

    public void UpdateDonGia(decimal donGia)
    {
        DonGia = new GiaTien(donGia);
    }
}
