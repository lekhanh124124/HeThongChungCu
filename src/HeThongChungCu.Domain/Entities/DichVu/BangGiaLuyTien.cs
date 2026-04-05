using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaLuyTien : BaseEntity
{
    public int BangGiaId { get; private set; }
    public decimal TuMuc { get; private set; }
    public decimal? DenMuc { get; private set; }
    public GiaTien DonGia { get; private set; } = null!;

    private BangGiaLuyTien() { } // EF Core

    internal BangGiaLuyTien(int bangGiaId, decimal tuMuc, decimal? denMuc, decimal donGia)
    {
        if (denMuc.HasValue && denMuc.Value <= tuMuc)
            throw new BusinessException("Đến số phải lớn hơn Từ số.");

        BangGiaId = bangGiaId;
        TuMuc = tuMuc;
        DenMuc = denMuc;
        DonGia = new GiaTien(donGia);
    }
}
