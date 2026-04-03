using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public abstract class YeuCau : AggregateRoot
{
    public int CanHoId { get; protected set; }
    public LoaiYeuCau LoaiYeuCauId { get; protected set; } = null!;
    public TrangThaiYeuCau TrangThaiId { get; protected set; } = null!;
    public string? NoiDung { get; protected set; }
    public string? LyDo { get; protected set; }
    public int? NguoiXuLyId { get; private set; }
    public int? NhanVienThucHienId { get; private set; }
    public DateTimeOffset? NgayXuLy { get; private set; }

    protected YeuCau() { } // EF Core

    protected YeuCau(int canHoId, LoaiYeuCau loaiYeuCau, string? noiDung = null, TrangThaiYeuCau? initialStatus = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauId = loaiYeuCau;
        NoiDung = noiDung;
        TrangThaiId = initialStatus ?? TrangThaiYeuCau.Pending;
    }

    public virtual void Approve(int adminId, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public virtual void Reject(int adminId, string lyDo, DateTimeOffset processedAt)
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

    public virtual void Withdraw()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể thu hồi yêu cầu đang ở trạng thái đã lưu hoặc đang chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
    }

    public virtual void Submit()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Withdrawn)
            throw new BusinessException("Chỉ có thể gửi yêu cầu đang ở trạng thái đã lưu hoặc đã thu hồi.");

        TrangThaiId = TrangThaiYeuCau.Pending;
    }

    public virtual void AssignStaff(int staffId)
    {
        if (TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể giao việc cho yêu cầu đã được duyệt.");

        NhanVienThucHienId = staffId;
    }
}
