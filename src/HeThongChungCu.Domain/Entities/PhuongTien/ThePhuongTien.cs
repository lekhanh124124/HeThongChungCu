using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ThePhuongTien : AuditableEntity
{
    public int PhuongTienId { get; private set; }
    public string MaThe { get; private set; } = string.Empty;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    
    public TrangThaiThePhuongTien TrangThaiId { get; private set; } = default!;

    public bool IsInUse => TrangThaiId == TrangThaiThePhuongTien.Active;

    private ThePhuongTien() { }

    internal ThePhuongTien(int phuongTienId, string maThe, DateTime ngayBatDau)
    {
        if (string.IsNullOrWhiteSpace(maThe))
            throw new BusinessException("Mã thẻ không được để trống.");

        PhuongTienId = phuongTienId;
        MaThe = maThe;
        NgayBatDau = ngayBatDau;
        TrangThaiId = TrangThaiThePhuongTien.Active;
    }

    public void KhoaThe(DateTime ngayKetThuc)
    {
        if (TrangThaiId == TrangThaiThePhuongTien.Locked || TrangThaiId == TrangThaiThePhuongTien.Lost)
            return;

        if (ngayKetThuc < NgayBatDau)
            throw new BusinessException("Ngày kết thúc không hợp lệ.");

        NgayKetThuc = ngayKetThuc;
        TrangThaiId = TrangThaiThePhuongTien.Locked;
    }

    public void BaoMat(DateTime now)
    {
        if (TrangThaiId == TrangThaiThePhuongTien.Lost)
            return;

        if (now < NgayBatDau)
            throw new BusinessException("Ngày báo mất không hợp lệ.");

        NgayKetThuc = now;
        TrangThaiId = TrangThaiThePhuongTien.Lost;
    }
}