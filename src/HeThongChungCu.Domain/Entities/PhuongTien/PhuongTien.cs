using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.PhuongTien;

public class PhuongTien : AggregateRoot
{
    public int CanHoId { get; private set; }
    public string TenPhuongTien { get; private set; } = null!;
    public LoaiPhuongTien LoaiPhuongTienId { get; private set; } = null!;
    public string BienSo { get; private set; } = null!;
    public string MauXe { get; private set; } = null!;

    private readonly List<ThePhuongTien> _thePhuongTiens = new();
    public IReadOnlyCollection<ThePhuongTien> ThePhuongTiens => _thePhuongTiens.AsReadOnly();

    private PhuongTien() { } // EF Core

    public PhuongTien(int canHoId, string tenPhuongTien, LoaiPhuongTien loaiPhuongTienId, string bienSo, string mauXe)
    {
        CanHoId = canHoId;
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
    }

    public void Update(string tenPhuongTien, LoaiPhuongTien loaiPhuongTienId, string bienSo, string mauXe)
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
