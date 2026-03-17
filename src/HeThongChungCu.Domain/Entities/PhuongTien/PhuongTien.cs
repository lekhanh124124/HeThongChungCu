using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

using HeThongChungCu.Domain.Policies;

namespace HeThongChungCu.Domain.Entities;

public class PhuongTien : AggregateRoot
{
    public int CanHoId { get; private set; }
    public string TenPhuongTien { get; private set; } = null!;
    public LoaiPhuongTien LoaiPhuongTienId { get; private set; } = null!;
    public string BienSo { get; private set; } = null!;
    public string MauXe { get; private set; } = null!;
    public TrangThaiPhuongTien TrangThaiPhuongTienId { get; private set; } = null!;

    private readonly List<ThePhuongTien> _thePhuongTiens = new();
    public IReadOnlyCollection<ThePhuongTien> ThePhuongTiens => _thePhuongTiens.AsReadOnly();

    private PhuongTien() { } // EF Core

    public PhuongTien(
        int canHoId, 
        string tenPhuongTien, 
        LoaiPhuongTien loaiPhuongTienId, 
        string bienSo, 
        string mauXe)
    {
        CanHoId = canHoId;
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
        TrangThaiPhuongTienId = TrangThaiPhuongTien.PendingApproval;
    }

    public void Update(
        string tenPhuongTien, 
        LoaiPhuongTien loaiPhuongTienId, 
        string bienSo, 
        string mauXe)
    {
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
    }

    public void UpdateStatus(TrangThaiPhuongTien trangThaiPhuongTienId)
    {
        TrangThaiPhuongTienId = trangThaiPhuongTienId;
    }

    public void AddThe(string maThe, DateTime ngayBatDau, IPhuongTienPolicy policy)
    {
        policy.ValidateAddThe(maThe, this);

        _thePhuongTiens.Add(new ThePhuongTien(Id, maThe, ngayBatDau));
    }
}
