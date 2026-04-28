using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaLuyTien : BangGia
{
    private readonly List<ChiTietGiaLuyTien> _chiTietGias = [];
    public IReadOnlyCollection<ChiTietGiaLuyTien> ChiTietGias => _chiTietGias.AsReadOnly();

    private BangGiaLuyTien() : base() { } // EF Core

    public BangGiaLuyTien(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        DateTimeOffset? ngayKetThuc = null)
        : base(dichVuId, tenBangGia, ngayApDung, LoaiDinhGia.LuyTien, ngayKetThuc)
    {
    }

    public void AddChiTietGia(decimal tuMuc, decimal? denMuc, decimal donGia)
    {
        var chiTietGia = new ChiTietGiaLuyTien(Id, tuMuc, denMuc, donGia);

        var previous = _chiTietGias.OrderBy(x => x.TuMuc).LastOrDefault();
        if (previous == null)
        {
            if (chiTietGia.TuMuc != 0)
                throw new BusinessException("Bậc đầu tiên phải bắt đầu từ 0.");
        }
        else
        {
            if (chiTietGia.TuMuc != previous.DenMuc)
                throw new BusinessException("Các bậc thang phải liên tục (không có khoảng trống hoặc chồng lấn).");
        }

        _chiTietGias.Add(chiTietGia);
    }

    public override decimal CalculateAmount(PricingContext context)
    {
        var consumption = context.SoLuong;
        decimal total = 0;

        foreach (var tier in _chiTietGias.OrderBy(x => x.TuMuc))
        {
            if (consumption <= tier.TuMuc) break;

            var amountInTier = (tier.DenMuc.HasValue ? Math.Min(consumption, tier.DenMuc.Value) : consumption) - tier.TuMuc;
            total += amountInTier * tier.DonGia.SoTien;

            if (tier.DenMuc.HasValue && consumption <= tier.DenMuc.Value) break;
        }

        return total;
    }
}
