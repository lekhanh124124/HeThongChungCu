using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.PhuongTien;

public class ThePhuongTien : AuditableEntity
{
    public int PhuongTienId { get; private set; }
    public string MaThe { get; private set; } = null!;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool IsLocked { get; private set; }

    private ThePhuongTien() { } // EF Core

    public ThePhuongTien(int phuongTienId, string maThe, DateTime ngayBatDau)
    {
        PhuongTienId = phuongTienId;
        MaThe = maThe;
        NgayBatDau = ngayBatDau;
        IsLocked = false;
    }

    public void KhoaThe(DateTime ngayKetThuc)
    {
        NgayKetThuc = ngayKetThuc;
        IsLocked = true;
    }
}
