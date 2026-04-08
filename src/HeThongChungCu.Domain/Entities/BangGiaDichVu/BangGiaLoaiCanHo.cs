using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

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
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, LoaiDinhGia.TheoDienTich, ngayKetThuc)
    {
    }

    public void AddGiaLoaiCanHo(LoaiCanHo? loaiCanHoId, decimal donGia)
    {
        if (_chiTietGias.Any(x => x.LoaiCanHoId == loaiCanHoId))
            throw new BusinessException("Loại căn hộ này đã có giá trong bảng giá này.");

        var detail = new ChiTietGiaLoaiCanHo(Id, loaiCanHoId, donGia);
        _chiTietGias.Add(detail);
    }
}
