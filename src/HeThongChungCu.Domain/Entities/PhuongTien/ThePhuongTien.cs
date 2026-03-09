using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.PhuongTien;

public class ThePhuongTien : AuditableEntity
{
    public int PhuongTienId { get; private set; }
    public string MaThe { get; private set; } = null!;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool TrangThai { get; private set; }

    private ThePhuongTien() { } // EF Core

    public ThePhuongTien(int phuongTienId, string maThe, DateTime ngayBatDau)
    {
        PhuongTienId = phuongTienId;
        MaThe = maThe;
        NgayBatDau = ngayBatDau;
        TrangThai = true;
    }

    public void KhoaThe(DateTime ngayKetThuc)
    {
        NgayKetThuc = ngayKetThuc;
        TrangThai = false;
    }
}
