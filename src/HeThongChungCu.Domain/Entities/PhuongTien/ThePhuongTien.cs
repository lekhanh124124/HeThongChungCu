using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ThePhuongTien : AuditableEntity
{
    public int PhuongTienId { get; private set; }
    public string MaThe { get; private set; } = string.Empty;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool IsLocked { get; private set; }

    private ThePhuongTien() { }

    internal ThePhuongTien(int phuongTienId, string maThe, DateTime ngayBatDau)
    {
        if (string.IsNullOrWhiteSpace(maThe))
            throw new BusinessException("Mã thẻ không được để trống.");

        PhuongTienId = phuongTienId;
        MaThe = maThe;
        NgayBatDau = ngayBatDau;
        IsLocked = false;
    }

    public void KhoaThe(DateTime ngayKetThuc)
    {
        if (IsLocked)
            throw new BusinessException("Thẻ đã bị khóa.");

        if (ngayKetThuc < NgayBatDau)
            throw new BusinessException("Ngày kết thúc không hợp lệ.");

        NgayKetThuc = ngayKetThuc;
        IsLocked = true;
    }
}