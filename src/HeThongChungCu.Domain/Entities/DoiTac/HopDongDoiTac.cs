using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class HopDongDoiTac : AuditableEntity
{
    public int DoiTacId { get; private set; }
    public virtual DoiTac DoiTac { get; private set; } = null!;
    public string SoHopDong { get; private set; } = string.Empty;
    public DateTimeOffset NgayKy { get; private set; }
    public DateTimeOffset NgayHetHan { get; private set; }
    public GiaTien GiaTriHopDong { get; private set; } = null!;
    public string? NoiDung { get; private set; }
    public int DichVuId { get; private set; }
    public TrangThaiHopDong TrangThaiHopDongId { get; private set; } = null!;
    
    private readonly List<TepHopDongDoiTac> _tepHopDongs = new();
    public virtual IReadOnlyCollection<TepHopDongDoiTac> TepHopDongs => _tepHopDongs.AsReadOnly();

    private HopDongDoiTac() { }

    public HopDongDoiTac(
        int doiTacId,
        string soHopDong,
        DateTimeOffset ngayKy,
        DateTimeOffset ngayHetHan,
        decimal giaTri,
        int dichVuId,
        string? noiDung = null)
    {
        if (string.IsNullOrWhiteSpace(soHopDong))
            throw new BusinessException("Số hợp đồng không được để trống.");

        if (ngayHetHan <= ngayKy)
            throw new BusinessException("Ngày hết hạn phải sau ngày ký.");

        DoiTacId = doiTacId;
        SoHopDong = soHopDong;
        NgayKy = ngayKy;
        NgayHetHan = ngayHetHan;
        GiaTriHopDong = new GiaTien(giaTri);
        DichVuId = dichVuId;
        NoiDung = noiDung;

        UpdateStatus();
    }

    public void SyncTepHopDongs(IEnumerable<TepHopDongDoiTac> teps)
    {
        foreach (var tep in _tepHopDongs)
        {
            tep.MarkAsUnused();
        }
        _tepHopDongs.Clear();
        foreach (var tep in teps)
        {
            tep.MarkAsUsed();
            _tepHopDongs.Add(tep);
        }
    }

    public void AddTepHopDong(TepHopDongDoiTac tep)
    {
        tep.MarkAsUsed();
        _tepHopDongs.Add(tep);
    }

    public void RemoveTepHopDong(int tepId)
    {
        var tep = _tepHopDongs.FirstOrDefault(x => x.Id == tepId);
        if (tep != null)
        {
            tep.MarkAsUnused();
            _tepHopDongs.Remove(tep);
        }
    }

    public void Revoke()
    {
        TrangThaiHopDongId = TrangThaiHopDong.DaThanhLy;
        if (NgayHetHan > DateTimeOffset.UtcNow)
        {
            NgayHetHan = DateTimeOffset.UtcNow;
        }
    }

    public void UpdateStatus()
    {
        var now = DateTimeOffset.UtcNow;
        if (now < NgayKy)
        {
            TrangThaiHopDongId = TrangThaiHopDong.ChuaKy;
        }
        else if (now > NgayHetHan)
        {
            TrangThaiHopDongId = TrangThaiHopDong.HetHan;
        }
        else if (NgayHetHan.AddMonths(-1) <= now)
        {
            TrangThaiHopDongId = TrangThaiHopDong.SapHetHan;
        }
        else
        {
            TrangThaiHopDongId = TrangThaiHopDong.ConHieuLuc;
        }
    }

    public bool IsActive() => TrangThaiHopDongId == TrangThaiHopDong.ConHieuLuc || TrangThaiHopDongId == TrangThaiHopDong.SapHetHan;
}
