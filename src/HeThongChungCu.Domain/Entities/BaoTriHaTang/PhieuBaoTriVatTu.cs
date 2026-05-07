using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class PhieuBaoTriVatTu : AuditableEntity
{
    public int PhieuBaoTriId { get; private set; }
    public string TenVatTu { get; private set; } = null!;
    public int SoLuong { get; private set; }
    public decimal DonGia { get; private set; }
    public decimal ThanhTien { get; private set; }

    private PhieuBaoTriVatTu() : base() { } // EF Core

    private PhieuBaoTriVatTu(string tenVatTu, int soLuong, decimal donGia) : base()
    {
        TenVatTu = tenVatTu;
        SoLuong = soLuong;
        DonGia = donGia;
        ThanhTien = soLuong * donGia;
    }

    public static PhieuBaoTriVatTu Create(string tenVatTu, int soLuong, decimal donGia)
    {
        return new PhieuBaoTriVatTu(tenVatTu, soLuong, donGia);
    }

    public void Update(string tenVatTu, int soLuong, decimal donGia)
    {
        TenVatTu = tenVatTu;
        SoLuong = soLuong;
        DonGia = donGia;
        ThanhTien = soLuong * donGia;
    }
}
