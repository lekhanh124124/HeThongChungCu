using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaCoDinh : BangGia
{
    public GiaTien DonGia { get; private set; } = null!;

    private BangGiaCoDinh() : base() { } // EF Core

    public BangGiaCoDinh(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        decimal donGia,
        LoaiDinhGia? loaiDinhGia = null,
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, loaiDinhGia ?? LoaiDinhGia.CoDinh, ngayKetThuc)
    {
        DonGia = new GiaTien(donGia);
    }
    public override decimal CalculateAmount(PricingContext context)
    {
        return DonGia.SoTien * context.SoLuong;
    }
}
