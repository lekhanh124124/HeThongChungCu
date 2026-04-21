using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauThiCong : YeuCau
{
    public string HangMucThiCong { get; private set; } = string.Empty;
    public DateTimeOffset DuKienBatDau { get; private set; }
    public DateTimeOffset DuKienKetThuc { get; private set; }

    public string? TenDonViThiCong { get; private set; }
    public string? NguoiDaiDien { get; private set; }
    public SoDienThoai? SoDienThoaiDaiDien { get; private set; }

    public decimal? TienDatCoc { get; private set; }
    public bool IsDaThuCoc { get; private set; }
    public string? GhiChuThuCoc { get; private set; }
    public decimal? TienKhauTru { get; private set; }
    public string? LyDoKhauTru { get; private set; }
    public bool IsDaHoanCoc { get; private set; }
    public bool IsYeuCauCoc => (TienDatCoc ?? 0) > 0;
    public DateTimeOffset? NgayDuyetSoBo { get; private set; }

    public TrangThaiThiCong? TrangThaiThiCongId { get; private set; }

    private readonly List<TepYeuCauThiCong> _tepYeuCauThiCongs = [];
    public IReadOnlyCollection<TepYeuCauThiCong> TepYeuCauThiCongs => _tepYeuCauThiCongs.AsReadOnly();

    private readonly List<NhanSuThiCong> _nhanSuThiCongs = [];
    public IReadOnlyCollection<NhanSuThiCong> NhanSuThiCongs => _nhanSuThiCongs.AsReadOnly();

    private YeuCauThiCong() : base() { } // EF Core

    private YeuCauThiCong(
        int canHoId,
        string hangMucThiCong,
        DateTimeOffset duKienBatDau,
        DateTimeOffset duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien,
        TrangThaiYeuCau? trangThaiBanDau = null)
        : base(canHoId, LoaiYeuCauCuDan.ThiCong, noiDung, trangThaiBanDau)
    {
        if (string.IsNullOrWhiteSpace(hangMucThiCong))
            throw new BusinessException("Cần cung cấp hạng mục thi công.");

        if (duKienKetThuc <= duKienBatDau)
            throw new BusinessException("Thời gian dự kiến thi công không hợp lệ.");

        HangMucThiCong = hangMucThiCong;
        DuKienBatDau = duKienBatDau;
        DuKienKetThuc = duKienKetThuc;
        TenDonViThiCong = tenDonViThiCong;
        NguoiDaiDien = nguoiDaiDien;
        SoDienThoaiDaiDien = new SoDienThoai(soDienThoaiDaiDien);
        TrangThaiThiCongId = TrangThaiThiCong.ChuaThiCong;
    }

    public static YeuCauThiCong Create(
        int canHoId,
        string hangMucThiCong,
        DateTimeOffset duKienBatDau,
        DateTimeOffset duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien,
        IEnumerable<TepYeuCauThiCong>? danhSachTep = null,
        TrangThaiYeuCau? trangThaiBanDau = null)
    {
        var request = new YeuCauThiCong(
            canHoId,
            hangMucThiCong,
            duKienBatDau,
            duKienKetThuc,
            noiDung,
            tenDonViThiCong,
            nguoiDaiDien,
            soDienThoaiDaiDien,
            trangThaiBanDau);

        if (danhSachTep != null)
        {
            foreach (var file in danhSachTep)
            {
                file.MarkAsUsed();
                request._tepYeuCauThiCongs.Add(file);
            }
        }

        return request;
    }

    public Result AddNhanSu(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null)
    {
        if (TrangThaiId == TrangThaiYeuCau.Completed || TrangThaiId == TrangThaiYeuCau.Cancelled)
            throw new BusinessException("Không thể bổ sung nhân sự cho yêu cầu đã kết thúc.");

        var staff = NhanSuThiCong.Create(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu);
        _nhanSuThiCongs.Add(staff);

        return Result.Success();
    }

    public Result RemoveNhanSu(int nhanSuId, string lyDo)
    {
        if (TrangThaiId == TrangThaiYeuCau.Completed || TrangThaiId == TrangThaiYeuCau.Cancelled)
            throw new BusinessException("Không thể xóa nhân sự cho yêu cầu đã kết thúc.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do xóa nhân sự để lưu lịch sử.");

        var staff = _nhanSuThiCongs.FirstOrDefault(x => x.Id == nhanSuId);
        if (staff != null)
        {
            staff.SetReasonForRemoval(lyDo);
            _nhanSuThiCongs.Remove(staff);
        }

        return Result.Success();
    }

    public Result CapNhatThongTinThiCong(
        string? hangMucThiCong,
        DateTimeOffset? duKienBatDau,
        DateTimeOffset? duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể chỉnh sửa khi yêu cầu đang ở trạng thái đã lưu hoặc yêu cầu bổ sung hồ sơ.");

        if (!string.IsNullOrWhiteSpace(hangMucThiCong))
            HangMucThiCong = hangMucThiCong;

        if (duKienBatDau != null)
            DuKienBatDau = duKienBatDau.Value;

        if (duKienKetThuc != null)
            DuKienKetThuc = duKienKetThuc.Value;

        if (DuKienKetThuc <= DuKienBatDau)
            throw new BusinessException("Thời gian dự kiến thi công không hợp lệ.");

        if (!string.IsNullOrWhiteSpace(noiDung))
            NoiDung = noiDung;

        TenDonViThiCong = tenDonViThiCong;
        NguoiDaiDien = nguoiDaiDien;
        SoDienThoaiDaiDien = new SoDienThoai(soDienThoaiDaiDien);

        // Lưu ý: Resident sẽ tự gọi Submit() sau khi hoàn tất chỉnh sửa để chuyển về Pending

        return Result.Success();
    }

    /// <summary>
    /// Trả lại yêu cầu - Giai đoạn 1.
    /// BQL yêu cầu cư dân bổ sung thông tin hoặc hồ sơ kỹ thuật.
    /// </summary>
    public override Result Return(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        var result = base.Return(adminId, lyDo, processedAt);
        if (result.IsFailure) return result;

        NgayDuyetSoBo = processedAt;
        TrangThaiThiCongId = TrangThaiThiCong.ChuaThiCong;

        return Result.Success();
    }

    /// <summary>
    /// Duyệt chính thức - Giai đoạn 2.
    /// Sau khi BQL đã trực tiếp xác minh hồ sơ và nhân sự với nhà thầu.
    /// </summary>
    public override Result Approve(int adminId, DateTimeOffset processedAt)
    {
        // Lưu ý: tienDatCoc cần được set trước hoặc thông qua một overload? 
        // Tuy nhiên, base.Approve không nhận decimal. 
        // Ta sẽ dùng lỗi BusinessException nếu TienDatCoc chưa được gán.
        if (TienDatCoc == null || TienDatCoc <= 0)
            throw new BusinessException("Cần xác định số tiền đặt cọc trước khi duyệt chính thức.");

        var result = base.Approve(adminId, processedAt);
        if (result.IsFailure) return result;

        TrangThaiThiCongId = TrangThaiThiCong.ChoThuCoc;

        return Result.Success();
    }

    /// <summary>
    /// Thiết lập tiền cọc trước khi duyệt.
    /// </summary>
    public void SetTienDatCoc(decimal amount)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending && TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể thiết lập tiền cọc khi yêu cầu đang chờ duyệt hoặc đã duyệt (chưa thu tiền).");

        if (IsDaThuCoc)
            throw new BusinessException("Không thể điều chỉnh tiền cọc sau khi đã xác nhận thu tiền.");

        if (amount < 0)
            throw new BusinessException("Tiền cọc không được âm.");
        TienDatCoc = amount;
    }

    public Result XacNhanThuCoc(string? ghiChu)
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.ChoThuCoc)
            throw new BusinessException("Yêu cầu chưa đến bước thu tiền cọc.");

        IsDaThuCoc = true;
        GhiChuThuCoc = ghiChu;
        TrangThaiThiCongId = TrangThaiThiCong.DaCapPhep;

        // Raise event
        AddDomainEvent(new YeuCauThiCongDaCapPhepEvent(this));

        return Result.Success();
    }


    public Result HoanTatThiCong()
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.DaCapPhep)
            throw new BusinessException("Chỉ có thể hoàn tất khi đã được cấp phép thi công.");

        TrangThaiThiCongId = TrangThaiThiCong.DaHoanTat;

        return Result.Success();
    }

    /// <summary>
    /// Xác nhận hoàn tiền đặt cọc (có khấu trừ nếu có).
    /// </summary>
    public Result HoanCoc(decimal khauTru, string? lyDo)
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.DaHoanTat)
            throw new BusinessException("Chỉ có thể hoàn cọc sau khi đã hoàn tất thi công và nghiệm thu.");

        if (IsDaHoanCoc)
            throw new BusinessException("Yêu cầu này đã được hoàn tiền cọc.");

        if (khauTru > TienDatCoc)
            throw new BusinessException("Số tiền khấu trừ không được vượt quá số tiền đã cọc.");

        TienKhauTru = khauTru;
        LyDoKhauTru = lyDo;
        IsDaHoanCoc = true;

        return Result.Success();
    }

    /// <summary>
    /// Đóng yêu cầu thi công sau khi hoàn tất.
    /// terminal: chuyển TrangThaiId sang Completed.
    /// </summary>
    public override Result Complete(int adminId, DateTimeOffset processedAt)
    {
        if (IsYeuCauCoc && !IsDaHoanCoc)
            throw new BusinessException("Cần thực hiện thủ tục hoàn tiền cọc trước khi đóng hồ sơ yêu cầu.");

        var result = base.Complete(adminId, processedAt);
        if (result.IsFailure) return result;

        // Giữ lại trạng thái DaHoanTat để lưu lịch sử nghiệm thu
        return Result.Success();
    }

    /// <summary>
    /// Hủy yêu cầu thi công.
    /// terminal: chuyển TrangThaiId sang Cancelled.
    /// </summary>
    public override Result Cancel(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        var result = base.Cancel(adminId, lyDo, processedAt);
        if (result.IsFailure) return result;

        TrangThaiThiCongId = null;

        return Result.Success();
    }


    public Result AddTep(TepYeuCauThiCong tep)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể bổ sung tài liệu khi yêu cầu đang ở trạng thái đã lưu hoặc yêu cầu bổ sung hồ sơ.");

        tep.MarkAsUsed();
        _tepYeuCauThiCongs.Add(tep);

        return Result.Success();
    }

    public Result RemoveTep(int tepId)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể xóa tài liệu khi yêu cầu đang ở trạng thái đã lưu hoặc yêu cầu bổ sung hồ sơ.");

        var tep = _tepYeuCauThiCongs.FirstOrDefault(x => x.Id == tepId);
        if (tep != null)
        {
            _tepYeuCauThiCongs.Remove(tep);
        }

        return Result.Success();
    }
}
