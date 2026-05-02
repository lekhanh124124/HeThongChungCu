using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaLoaiCanHo : BangGia
{
    private readonly List<ChiTietGiaLoaiCanHo> _chiTietGias = [];
    public IReadOnlyCollection<ChiTietGiaLoaiCanHo> ChiTietGias => _chiTietGias.AsReadOnly();

    private BangGiaLoaiCanHo() : base() { } // EF Core

    public BangGiaLoaiCanHo(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        bool isDinhKy,
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, LoaiDinhGia.TheoDienTich, isDinhKy, ngayKetThuc)
    {
    }

    public void AddGiaLoaiCanHo(LoaiCanHo? loaiCanHoId, decimal donGia)
    {
        if (_chiTietGias.Any(x => x.LoaiCanHoId == loaiCanHoId))
            throw new BusinessException("Loại căn hộ này đã có giá trong bảng giá này.");

        var detail = new ChiTietGiaLoaiCanHo(Id, loaiCanHoId, donGia);
        _chiTietGias.Add(detail);
    }

    public override decimal CalculateAmount(PricingContext context)
    {
        var detail = _chiTietGias.FirstOrDefault(x => x.LoaiCanHoId == context.LoaiCanHoId);
        if (detail == null) return 0;

        // Tính toán số tiền dựa trên số lượng (thường là diện tích do Service truyền vào)
        return detail.DonGia.SoTien * context.SoLuong;
    }
}
