using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
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
    public DateTimeOffset? NgayXuLy { get; private set; }

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
        TrangThaiYeuCau trangThaiId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung = null,
        int? phuongTienId = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauId = loaiYeuCau;
        TrangThaiId = trangThaiId;
        YeuCauLoaiPhuongTienId = loaiPhuongTien;
        YeuCauTenPhuongTien = tenPhuongTien;
        YeuCauBienSo = bienSo;
        YeuCauMauXe = mauXe;
        NoiDung = noiDung;
        YeuCauPhuongTienId = phuongTienId;
    }

    public static YeuCauPhuongTien CreateAddRequest(
        int canHoId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Them, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung);

        if (images != null)
        {
            foreach (var img in images)
            {
                img.MarkAsUsed();
                request._yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }

    public static YeuCauPhuongTien CreateUpdateRequest(
        int canHoId,
        int phuongTienId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Sua, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung, phuongTienId);

        if (images != null)
        {
            foreach (var img in images)
            {
                img.MarkAsUsed();
                request._yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }

    public static YeuCauPhuongTien CreateDeleteRequest(
        int canHoId,
        int phuongTienId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Xoa, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung, phuongTienId);
        
        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }

    public void Approve(int adminId, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public void Reject(int adminId, string lyDo, DateTimeOffset processedAt)
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

    public void Update(
        LoaiPhuongTien? loaiPhuongTien,
        string? tenPhuongTien,
        string? bienSo,
        string? mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể chỉnh sửa yêu cầu đang ở trạng thái đã lưu.");

        if (loaiPhuongTien != null) YeuCauLoaiPhuongTienId = loaiPhuongTien;
        if (!string.IsNullOrEmpty(tenPhuongTien)) YeuCauTenPhuongTien = tenPhuongTien;
        if (!string.IsNullOrEmpty(bienSo)) YeuCauBienSo = bienSo;
        if (!string.IsNullOrEmpty(mauXe)) YeuCauMauXe = mauXe;
        NoiDung = noiDung;

        if (images != null)
        {
            _yeuCauHinhAnhPhuongTiens.Clear();
            foreach (var img in images)
            {
                img.MarkAsUsed();
                _yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }
    }

    public void Submit()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Withdrawn)
            throw new BusinessException("Chỉ có thể gửi yêu cầu đang ở trạng thái đã lưu hoặc đã thu hồi.");

        TrangThaiId = TrangThaiYeuCau.Pending;
        AddDomainEvent(new YeuCauPhuongTienCreatedEvent(this));
    }

    public void Withdraw()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể thu hồi yêu cầu đang ở trạng thái đã lưu hoặc đang chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
    }
}
