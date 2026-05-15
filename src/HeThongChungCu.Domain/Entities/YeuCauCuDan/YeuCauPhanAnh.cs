using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauPhanAnh : YeuCau
{
    public string TieuDe { get; private set; } = null!;
    public LoaiPhanAnh LoaiPhanAnhId { get; private set; } = null!;
    public TrangThaiPhanAnh TrangThaiPhanAnhId { get; private set; } = null!;
    public DateTimeOffset? HanPhanHoi { get; private set; }
    public bool IsQuaHanNotified { get; private set; }

    public int? DiemDanhGia { get; private set; } // 1 - 5 sao
    public string? NhanXetDanhGia { get; private set; }
    public DateTimeOffset? NgayDanhGia { get; private set; }

    private readonly List<TraLoiPhanAnh> _traLoiPhanAnhs = [];
    public IReadOnlyCollection<TraLoiPhanAnh> TraLoiPhanAnhs => _traLoiPhanAnhs.AsReadOnly();

    private readonly List<TepYeuCauPhanAnh> _tepYeuCauPhanAnhs = [];
    public IReadOnlyCollection<TepYeuCauPhanAnh> TepYeuCauPhanAnhs => _tepYeuCauPhanAnhs.AsReadOnly();

    private YeuCauPhanAnh() : base() { } // EF Core

    private YeuCauPhanAnh(
        int canHoId,
        string tieuDe,
        string noiDung,
        LoaiPhanAnh loaiPhanAnh,
        bool isSubmit)
        : base(canHoId, LoaiYeuCauCuDan.PhanAnh, noiDung, isSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved)
    {
        TieuDe = tieuDe;
        LoaiPhanAnhId = loaiPhanAnh;
        TrangThaiPhanAnhId = isSubmit ? TrangThaiPhanAnh.ChoTiepNhan : TrangThaiPhanAnh.Nhap;
    }

    public static Result<YeuCauPhanAnh> Create(
        int canHoId,
        string tieuDe,
        string noiDung,
        LoaiPhanAnh loaiPhanAnh,
        IEnumerable<TepYeuCauPhanAnh>? teps = null,
        bool isSubmit = true)
    {
        if (string.IsNullOrWhiteSpace(tieuDe) || string.IsNullOrWhiteSpace(noiDung))
            return Result.Failure<YeuCauPhanAnh>(PhanAnhErrors.EmptyTitleOrContent);

        var phanAnh = new YeuCauPhanAnh(canHoId, tieuDe, noiDung, loaiPhanAnh, isSubmit);

        if (teps != null)
        {
            foreach (var tep in teps)
            {
                tep.MarkAsUsed();
                phanAnh._tepYeuCauPhanAnhs.Add(tep);
            }
        }

        if (isSubmit)
        {
            phanAnh.HanPhanHoi = DateTimeOffset.Now.AddHours(loaiPhanAnh.HanXuLyGio);
            phanAnh.AddDomainEvent(new YeuCauPhanAnhCreatedEvent(phanAnh));
        }

        return Result.Success(phanAnh);
    }

    public Result TiepNhanVaPhanCong(int adminId, DateTimeOffset processedAt)
    {
        if (TrangThaiPhanAnhId != TrangThaiPhanAnh.ChoTiepNhan)
            return Result.Failure(PhanAnhErrors.InvalidStatus);

        TrangThaiPhanAnhId = TrangThaiPhanAnh.DangXuLy;

        // Đồng bộ trạng thái cơ bản của YeuCau sang Approved (Đã phê duyệt tiếp nhận)
        var approveResult = Approve(adminId, processedAt);
        if (approveResult.IsFailure)
            return approveResult;

        return Result.Success();
    }

    public Result Update(
        string? tieuDe,
        string? noiDung,
        LoaiPhanAnh? loaiPhanAnh,
        IEnumerable<TepYeuCauPhanAnh>? documents)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Withdrawn)
            return Result.Failure(new Error("PhanAnh.InvalidStatusForUpdate", "Chỉ có thể chỉnh sửa yêu cầu phản ánh đang ở trạng thái Nháp hoặc Đã thu hồi."));

        if (tieuDe != null)
        {
            if (string.IsNullOrWhiteSpace(tieuDe))
                return Result.Failure(PhanAnhErrors.EmptyTitleOrContent);
            TieuDe = tieuDe;
        }

        if (noiDung != null)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                return Result.Failure(PhanAnhErrors.EmptyTitleOrContent);
            NoiDung = noiDung;
        }

        if (loaiPhanAnh != null)
        {
            LoaiPhanAnhId = loaiPhanAnh;
        }

        if (documents != null)
        {
            foreach (var tep in _tepYeuCauPhanAnhs)
            {
                tep.MarkAsUnused();
            }
            _tepYeuCauPhanAnhs.Clear();
            foreach (var tep in documents)
            {
                tep.MarkAsUsed();
                _tepYeuCauPhanAnhs.Add(tep);
            }
        }

        return Result.Success();
    }

    public override Result Submit()
    {
        var result = base.Submit(); // Chuyển TrangThaiId -> Pending
        if (result.IsFailure) return result;

        TrangThaiPhanAnhId = TrangThaiPhanAnh.ChoTiepNhan;
        HanPhanHoi = DateTimeOffset.Now.AddHours(LoaiPhanAnhId.HanXuLyGio);
        AddDomainEvent(new YeuCauPhanAnhCreatedEvent(this));
        return Result.Success();
    }

    public override Result Withdraw()
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending &&
            TrangThaiId != TrangThaiYeuCau.Saved &&
            TrangThaiId != TrangThaiYeuCau.Returned)
        {
            return Result.Failure(new Error("PhanAnh.InvalidStatusForWithdraw", "Chỉ có thể thu hồi yêu cầu phản ánh ở trạng thái Chờ tiếp nhận, Nháp hoặc Yêu cầu bổ sung."));
        }

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
        TrangThaiPhanAnhId = TrangThaiPhanAnh.DaThuHoi;
        return Result.Success();
    }

    public Result ThemPhanHoi(string noiDung, bool isNhanVien)
    {
        if (TrangThaiPhanAnhId == TrangThaiPhanAnh.DaDong || TrangThaiPhanAnhId == TrangThaiPhanAnh.DaHuy)
            return Result.Failure(PhanAnhErrors.InvalidStatus);

        if (string.IsNullOrWhiteSpace(noiDung))
            return Result.Failure(PhanAnhErrors.EmptyComment);

        var traLoi = TraLoiPhanAnh.Create(noiDung, isNhanVien);
        _traLoiPhanAnhs.Add(traLoi);

        // Tự động chuyển đổi trạng thái tương tác dựa vào người gửi phản hồi
        TrangThaiPhanAnhId = isNhanVien ? TrangThaiPhanAnh.CSKHPhanHoi : TrangThaiPhanAnh.CuDanPhanHoi;

        return Result.Success();
    }

    public Result XacNhanHoanThanh(int adminId, string ketQua, DateTimeOffset processedAt)
    {
        if (TrangThaiPhanAnhId != TrangThaiPhanAnh.DangXuLy &&
            TrangThaiPhanAnhId != TrangThaiPhanAnh.CSKHPhanHoi &&
            TrangThaiPhanAnhId != TrangThaiPhanAnh.CuDanPhanHoi)
        {
            return Result.Failure(PhanAnhErrors.InvalidStatus);
        }

        TrangThaiPhanAnhId = TrangThaiPhanAnh.ChoDanhGia;
        NgayXuLy = processedAt;

        var thongBao = TraLoiPhanAnh.Create($"[HỆ THỐNG]: BQL xác nhận đã xử lý xong phản ánh với kết quả: {ketQua}", true);
        _traLoiPhanAnhs.Add(thongBao);

        return Result.Success();
    }

    public Result CuDanDanhGiaVaDongTicket(int diem, string? nhanXet)
    {
        if (TrangThaiPhanAnhId != TrangThaiPhanAnh.ChoDanhGia)
            return Result.Failure(PhanAnhErrors.InvalidStatus);

        if (diem < 1 || diem > 5)
            return Result.Failure(PhanAnhErrors.InvalidRating);

        DiemDanhGia = diem;
        NhanXetDanhGia = nhanXet;
        NgayDanhGia = DateTimeOffset.Now;

        TrangThaiPhanAnhId = TrangThaiPhanAnh.DaDong;

        // Đồng bộ trạng thái cơ sở của YeuCau sang Completed
        var completeResult = Complete(NguoiXuLyId ?? 0, DateTimeOffset.Now);
        if (completeResult.IsFailure)
            return completeResult;

        return Result.Success();
    }

    public override Result Cancel(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        if (TrangThaiPhanAnhId == TrangThaiPhanAnh.DaDong || TrangThaiPhanAnhId == TrangThaiPhanAnh.DaHuy)
            return Result.Failure(new Error("PhanAnh.InvalidStatusForCancel", "Không thể hủy phản ánh đã hoàn thành hoặc đã bị hủy trước đó."));

        TrangThaiPhanAnhId = TrangThaiPhanAnh.DaHuy;

        var cancelResult = base.Cancel(adminId, lyDo, processedAt);
        if (cancelResult.IsFailure)
            return cancelResult;

        var thongBao = TraLoiPhanAnh.Create($"[HỆ THỐNG]: BQL đã hủy/từ chối phản ánh này với lý do: {lyDo}", true);
        _traLoiPhanAnhs.Add(thongBao);

        return Result.Success();
    }

    public void SetHanPhanHoi(DateTimeOffset hanPhanHoi)
    {
        HanPhanHoi = hanPhanHoi;
    }

    public void MarkAsOverdueNotified()
    {
        IsQuaHanNotified = true;
    }
}
