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

    private PhuongTien() { }

    public PhuongTien(
        int canHoId,
        string tenPhuongTien,
        LoaiPhuongTien loaiPhuongTienId,
        string bienSo,
        string mauXe)
    {
        if (string.IsNullOrWhiteSpace(bienSo))
            throw new BusinessException("Biển số không được để trống.");

        CanHoId = canHoId;
        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;

        TrangThaiPhuongTienId = TrangThaiPhuongTien.PendingApproval;
    }

    public void UpdateTrangThai(TrangThaiPhuongTien trangThai)
    {
        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Disabled && trangThai != TrangThaiPhuongTien.Disabled)
            throw new BusinessException("Phương tiện đã bị vô hiệu, không thể chuyển sang trạng thái khác.");

        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Approved && trangThai == TrangThaiPhuongTien.PendingApproval)
            throw new BusinessException("Không thể chuyển phương tiện đã duyệt về chờ duyệt.");
            
        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Rejected && trangThai != TrangThaiPhuongTien.Rejected)
            throw new BusinessException("Phương tiện đã bị từ chối, không thể chuyển sang trạng thái khác.");

        TrangThaiPhuongTienId = trangThai;

        if (trangThai == TrangThaiPhuongTien.Disabled)
        {
            // Lock tất cả thẻ
            foreach (var the in _thePhuongTiens.Where(x => !x.IsLocked))
            {
                the.KhoaThe(DateTime.Now);
            }
        }
    }

    public void UpdateInfo(
        string tenPhuongTien,
        LoaiPhuongTien loaiPhuongTienId,
        string bienSo,
        string mauXe)
    {
        if (TrangThaiPhuongTienId == TrangThaiPhuongTien.Approved)
            throw new BusinessException("Không được sửa phương tiện đã duyệt.");

        TenPhuongTien = tenPhuongTien;
        LoaiPhuongTienId = loaiPhuongTienId;
        BienSo = bienSo;
        MauXe = mauXe;
    }

    public ThePhuongTien AddThe(string maThe, DateTime ngayBatDau)
    {
        if (TrangThaiPhuongTienId != TrangThaiPhuongTien.Approved)
            throw new BusinessException("Chỉ phương tiện đã duyệt mới được cấp thẻ.");

        if (_thePhuongTiens.Any(x => !x.IsLocked))
            throw new BusinessException("Phương tiện đã có thẻ đang hoạt động.");

        var the = new ThePhuongTien(Id, maThe, ngayBatDau);

        _thePhuongTiens.Add(the);

        return the;
    }

    public void KhoaThe(int theId)
    {
        var the = _thePhuongTiens.FirstOrDefault(x => x.Id == theId);
        if (the == null)
            throw new BusinessException("Không tìm thấy thẻ phương tiện.");

        the.KhoaThe(DateTime.Now);
    }

    public void Xoa()
    {
        foreach (var the in _thePhuongTiens.Where(x => !x.IsLocked))
        {
            the.KhoaThe(DateTime.Now);
        }
    }
}