using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public abstract class YeuCau : AggregateRoot
{
    public LoaiYeuCauCuDan LoaiYeuCauCuDanId { get; protected set; } = null!;
    public int CanHoId { get; protected set; }
    public TrangThaiYeuCau TrangThaiId { get; protected set; } = null!;
    public string? NoiDung { get; protected set; }
    public string? LyDo { get; protected set; }
    public int? NguoiXuLyId { get; protected set; }
    public DateTimeOffset? NgayXuLy { get; protected set; }

    protected YeuCau() { } // EF Core

    protected YeuCau(int canHoId, LoaiYeuCauCuDan loaiYeuCauCuDan, string? noiDung = null, TrangThaiYeuCau? initialStatus = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauCuDanId = loaiYeuCauCuDan;
        NoiDung = noiDung;
        TrangThaiId = initialStatus ?? TrangThaiYeuCau.Pending;
    }

    public virtual Result Approve(int adminId, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;

        return Result.Success();
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

    public virtual Result Return(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể yêu cầu bổ sung thông tin cho yêu cầu đang chờ duyệt.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do yêu cầu bổ sung.");

        TrangThaiId = TrangThaiYeuCau.Returned;
        LyDo = lyDo;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;

        return Result.Success();
    }

    public virtual void Invalidate(int adminId, string? lyDo, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending && TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            return;

        TrangThaiId = TrangThaiYeuCau.Invalidated;
        LyDo = string.IsNullOrWhiteSpace(LyDo) ? lyDo : $"{LyDo} | {lyDo}";
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public virtual void Withdraw()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể thu hồi yêu cầu đang ở trạng thái nháp (Saved) hoặc yêu cầu bổ sung (Returned).");

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
    }

    public virtual void Submit()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể gửi yêu cầu đang ở trạng thái nháp (Saved) hoặc được yêu cầu bổ sung (Returned).");

        TrangThaiId = TrangThaiYeuCau.Pending;
    }

}
