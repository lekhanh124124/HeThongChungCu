using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauPhuongTien : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int? YeuCauPhuongTienId { get; private set; } // Liên kết tới xe cũ nếu là yêu cầu Sửa/Xóa

    public LoaiYeuCau LoaiYeuCauId { get; private set; } = null!;
    public TrangThaiYeuCau TrangThaiId { get; private set; } = null!;

    public string? NoiDung { get; private set; }
    public string? LyDo { get; private set; }

    public int? NguoiXuLyId { get; private set; }
    public DateTime? NgayXuLy { get; private set; }

    // Thông tin xe đề xuất
    public string YeuCauTenPhuongTien { get; private set; } = string.Empty;
    public LoaiPhuongTien YeuCauLoaiPhuongTienId { get; private set; } = default!;
    public string YeuCauBienSo { get; private set; } = string.Empty;
    public string YeuCauMauXe { get; private set; } = string.Empty;

    private readonly List<TepTaiLieu> _yeuCauHinhAnhPhuongTiens = [];
    public IReadOnlyCollection<TepTaiLieu> YeuCauHinhAnhPhuongTiens => _yeuCauHinhAnhPhuongTiens.AsReadOnly();

    private YeuCauPhuongTien() { } // EF Core

    private YeuCauPhuongTien(
        int canHoId,
        LoaiYeuCau loaiYeuCau,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung = null,
        int? phuongTienId = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauId = loaiYeuCau;
        YeuCauLoaiPhuongTienId = loaiPhuongTien;
        YeuCauTenPhuongTien = tenPhuongTien;
        YeuCauBienSo = bienSo;
        YeuCauMauXe = mauXe;
        NoiDung = noiDung;
        YeuCauPhuongTienId = phuongTienId;
        TrangThaiId = TrangThaiYeuCau.Pending;
    }

    public static YeuCauPhuongTien CreateAddRequest(
        int canHoId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Them, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung);

        if (images != null)
        {
            foreach (var img in images)
            {
                img.MarkAsUsed();
                request._yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }

        return request;
    }

    public void Approve(int adminId, DateTime processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public void Reject(int adminId, string lyDo, DateTime processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể từ chối yêu cầu đang ở trạng thái chờ duyệt.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do từ chối.");

        TrangThaiId = TrangThaiYeuCau.Rejected;
        LyDo = lyDo;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }
}
