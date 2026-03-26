using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class BangGiaLuyTien : BaseEntity
{
    public int BangGiaId { get; private set; }
    public double TuMuc { get; private set; }
    public double? DenMuc { get; private set; }
    public decimal DonGia { get; private set; }

    private BangGiaLuyTien() { } // EF Core

    internal BangGiaLuyTien(int bangGiaId, double tuMuc, double? denMuc, decimal donGia)
    {
        if (denMuc.HasValue && denMuc.Value <= tuMuc)
            throw new BusinessException("Đến số phải lớn hơn Từ số.");
        
        if (donGia < 0)
            throw new BusinessException("Đơn giá không được âm.");

        BangGiaId = bangGiaId;
        TuMuc = tuMuc;
        DenMuc = denMuc;
        DonGia = donGia;
    }
}