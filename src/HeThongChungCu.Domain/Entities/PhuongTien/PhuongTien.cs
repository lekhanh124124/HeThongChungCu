using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.PhuongTien;

public class PhuongTien : AggregateRoot
{
    public int CanHoId { get; private set; }
    public string TenPhuongTien { get; private set; } = null!;
    public int LoaiPhuongTienId { get; private set; }
    public string BienSo { get; private set; } = null!;
    public string MauXe { get; private set; } = null!;

    private readonly List<ThePhuongTien> _thePhuongTiens = new();
    public IReadOnlyCollection<ThePhuongTien> ThePhuongTiens => _thePhuongTiens.AsReadOnly();

    private PhuongTien() { } // EF Core

    public PhuongTien(int canHoId, string tenPhuongTien, int loaiPhuongTienId, string bienSo, string mauXe)
    {
        CanHoId = canHoId;
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
    }

    public void Update(string tenPhuongTien, int loaiPhuongTienId, string bienSo, string mauXe)
    {
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
    }

    public void AddThe(string maThe, DateTime ngayBatDau)
    {
        _thePhuongTiens.Add(new ThePhuongTien(Id, maThe, ngayBatDau));
    }
}
