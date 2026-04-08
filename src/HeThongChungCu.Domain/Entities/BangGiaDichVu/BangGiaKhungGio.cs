using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

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
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, LoaiDinhGia.TheoKhungGio, ngayKetThuc)
    {
    }

    public void AddGiaKhungGio(int khungGioId, decimal donGia)
    {
        if (_chiTietGias.Any(x => x.KhungGioId == khungGioId))
            throw new BusinessException("Khung giờ đã có giá trong bảng giá này.");

        var detail = new ChiTietGiaKhungGio(Id, khungGioId, donGia);
        _chiTietGias.Add(detail);
    }
}
