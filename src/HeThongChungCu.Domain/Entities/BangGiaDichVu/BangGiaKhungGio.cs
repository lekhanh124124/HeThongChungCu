using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaKhungGio : BangGia
{
    private readonly List<ChiTietGiaKhungGio> _chiTietGias = [];
    public IReadOnlyCollection<ChiTietGiaKhungGio> ChiTietGias => _chiTietGias.AsReadOnly();

    private BangGiaKhungGio() : base() { } // EF Core

    public BangGiaKhungGio(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        bool isDinhKy,
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, LoaiDinhGia.TheoKhungGio, isDinhKy, ngayKetThuc)
    {
    }

    public void AddGiaKhungGio(int khungGioId, decimal donGia)
    {
        if (_chiTietGias.Any(x => x.KhungGioId == khungGioId))
            throw new BusinessException("Khung giờ đã có giá trong bảng giá này.");

        var detail = new ChiTietGiaKhungGio(Id, khungGioId, donGia);
        _chiTietGias.Add(detail);
    }

    public override decimal CalculateAmount(PricingContext context)
    {
        var detail = _chiTietGias.FirstOrDefault(x => x.KhungGioId == context.KhungGioId);
        if (detail == null) return 0;

        return detail.DonGia.SoTien * context.SoLuong;
    }
}
