using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class PhuongTien : AggregateRoot
{
    public int CanHoId { get; private set; }

    public string TenPhuongTien { get; private set; } = string.Empty;
    public LoaiPhuongTien LoaiPhuongTienId { get; private set; } = default!;
    public string BienSo { get; private set; } = string.Empty;
    public string MauXe { get; private set; } = string.Empty;

    public TrangThaiPhuongTien TrangThaiPhuongTienId { get; private set; } = default!;

    private readonly List<ThePhuongTien> _thePhuongTiens = new();
    public IReadOnlyCollection<ThePhuongTien> ThePhuongTiens => _thePhuongTiens.AsReadOnly();

    private readonly List<TepPhuongTien> _hinhAnhPhuongTiens = new();
    public IReadOnlyCollection<TepPhuongTien> HinhAnhPhuongTiens => _hinhAnhPhuongTiens.AsReadOnly();

    private PhuongTien() { }

    public PhuongTien(
        int canHoId,
        string tenPhuongTien,
        LoaiPhuongTien loaiPhuongTienId,
        string bienSo,
        string mauXe,
        IEnumerable<TepPhuongTien>? hinhAnhs = null)
    {
        if (string.IsNullOrWhiteSpace(bienSo))
            throw new BusinessException("Biển số không được để trống.");

        CanHoId = canHoId;
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;

        TrangThaiPhuongTienId = TrangThaiPhuongTien.Active;

        if (hinhAnhs != null)
        {
            foreach (var hinhAnh in _hinhAnhPhuongTiens)
            {
                hinhAnh.MarkAsUnused();
            }
            _hinhAnhPhuongTiens.Clear();
            foreach (var hinhAnh in hinhAnhs)
            {
                hinhAnh.MarkAsUsed();
                _hinhAnhPhuongTiens.Add(hinhAnh);
            }
        }
    }

    /// <summary>
    /// Kích hoạt phương tiện dựa trên hạn mức cứng.
    /// </summary>
    public void Activate()
    {
        TrangThaiPhuongTienId = TrangThaiPhuongTien.Active;
    }

    public void Huy(DateTimeOffset now)
    {
        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Inactive)
            return;

        TrangThaiPhuongTienId = TrangThaiPhuongTien.Inactive;

        // Lock tất cả thẻ
        foreach (var the in _thePhuongTiens.Where(x => x.IsInUse))
        {
            the.KhoaThe(now);
        }
    }

    public void Khoa(DateTimeOffset now)
    {
        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Blocked)
            return;

        TrangThaiPhuongTienId = TrangThaiPhuongTien.Blocked;

        // Lock tất cả thẻ
        foreach (var the in _thePhuongTiens.Where(x => x.IsInUse))
        {
            the.KhoaThe(now);
        }
    }

    public void CapNhat(
        string tenPhuongTien,
        LoaiPhuongTien loaiPhuongTienId,
        string bienSo,
        string mauXe,
        IEnumerable<TepPhuongTien>? hinhAnhs = null)
    {
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;

        if (hinhAnhs != null)
        {
            // Clear old ones
            foreach (var old in _hinhAnhPhuongTiens)
            {
                old.MarkAsUnused();
            }
            _hinhAnhPhuongTiens.Clear();

            // Add new ones
            foreach (var hinhAnh in hinhAnhs)
            {
                hinhAnh.MarkAsUsed();
                _hinhAnhPhuongTiens.Add(hinhAnh);
            }
        }
    }

    public ThePhuongTien AddThe(string maThe, DateTimeOffset ngayBatDau)
    {
        if (TrangThaiPhuongTienId != TrangThaiPhuongTien.Active)
            throw new BusinessException("Chỉ phương tiện đang hoạt động mới được cấp thẻ.");

        if (_thePhuongTiens.Any(x => x.IsInUse))
            throw new BusinessException("Phương tiện vẫn còn thẻ đang hoạt động.");

        var the = new ThePhuongTien(Id, maThe, ngayBatDau);

        _thePhuongTiens.Add(the);

        return the;
    }

    public void KhoaThe(int theId, DateTimeOffset now)
    {
        var the = _thePhuongTiens.FirstOrDefault(x => x.Id == theId);
        if (the == null)
            throw new BusinessException("Không tìm thấy thẻ phương tiện.");

        the.KhoaThe(now);
    }

    public void BaoMatThe(int theId, DateTimeOffset now)
    {
        var the = _thePhuongTiens.FirstOrDefault(x => x.Id == theId);
        if (the == null)
            throw new BusinessException("Không tìm thấy thẻ phương tiện.");

        the.BaoMat(now);
    }
}