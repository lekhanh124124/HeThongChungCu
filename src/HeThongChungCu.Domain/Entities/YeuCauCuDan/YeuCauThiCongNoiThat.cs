using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauThiCongNoiThat : YeuCau
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
    public DateTimeOffset? NgayDuyetSoBo { get; private set; }

    public TrangThaiThiCong TrangThaiThiCongId { get; private set; } = null!;

    private readonly List<TepYeuCauThiCongNoiThat> _tepYeuCauThiCongNoiThats = [];
    public IReadOnlyCollection<TepYeuCauThiCongNoiThat> TepYeuCauThiCongNoiThats => _tepYeuCauThiCongNoiThats.AsReadOnly();

    private readonly List<NhanSuThiCong> _nhanSuThiCongs = [];
    public IReadOnlyCollection<NhanSuThiCong> NhanSuThiCongs => _nhanSuThiCongs.AsReadOnly();

    private YeuCauThiCongNoiThat() : base() { } // EF Core

    private YeuCauThiCongNoiThat(
        int canHoId,
        string hangMucThiCong,
        DateTimeOffset duKienBatDau,
        DateTimeOffset duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien,
        TrangThaiYeuCau? trangThaiBanDau = null)
        : base(canHoId, LoaiYeuCauCuDan.ThiCongNoiThat, noiDung, trangThaiBanDau)
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

    public static YeuCauThiCongNoiThat Create(
        int canHoId,
        string hangMucThiCong,
        DateTimeOffset duKienBatDau,
        DateTimeOffset duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien,
        IEnumerable<TepYeuCauThiCongNoiThat>? danhSachTep = null,
        TrangThaiYeuCau? trangThaiBanDau = null)
    {
        var request = new YeuCauThiCongNoiThat(
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
                request._tepYeuCauThiCongNoiThats.Add(file);
            }
        }

        return request;
    }

    public void AddNhanSu(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null)
    {
        if (TrangThaiThiCongId == TrangThaiThiCong.DaDong || TrangThaiThiCongId == TrangThaiThiCong.DaHuy)
            throw new BusinessException("Không thể bổ sung nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        // Root trực tiếp khởi tạo con
        var staff = NhanSuThiCong.Create(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu);
        _nhanSuThiCongs.Add(staff);
    }

    public void UpdateNhanSu(int nhanSuId, string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null)
    {
        if (TrangThaiThiCongId == TrangThaiThiCong.DaDong || TrangThaiThiCongId == TrangThaiThiCong.DaHuy)
            throw new BusinessException("Không thể cập nhật nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        var staff = _nhanSuThiCongs.FirstOrDefault(x => x.Id == nhanSuId);
        if (staff == null)
            throw new BusinessException("Không tìm thấy thông tin nhân sự để cập nhật.");

        staff.UpdateInfo(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu);
    }

    public void RemoveNhanSu(int nhanSuId)
    {
        if (TrangThaiThiCongId == TrangThaiThiCong.DaDong || TrangThaiThiCongId == TrangThaiThiCong.DaHuy)
            throw new BusinessException("Không thể xóa nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        var staff = _nhanSuThiCongs.FirstOrDefault(x => x.Id == nhanSuId);
        if (staff != null)
        {
            _nhanSuThiCongs.Remove(staff);
        }
    }

    public void CapNhatThongTinThiCong(
        string? hangMucThiCong,
        DateTimeOffset? duKienBatDau,
        DateTimeOffset? duKienKetThuc,
        string? noiDung,
        string? tenDonViThiCong,
        string? nguoiDaiDien,
        string? soDienThoaiDaiDien)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiThiCongId != TrangThaiThiCong.ChoBoSungHoSo)
            throw new BusinessException("Chỉ có thể chỉnh sửa khi yêu cầu đang ở trạng thái đã lưu hoặc chờ bổ sung hồ sơ.");

        if (!string.IsNullOrWhiteSpace(hangMucThiCong))
            HangMucThiCong = hangMucThiCong;

        if (duKienBatDau != null)
            DuKienBatDau = duKienBatDau.Value;

        if (duKienKetThuc != null)
            DuKienKetThuc = duKienKetThuc.Value;

        if (DuKienKetThuc <= DuKienBatDau)
            throw new BusinessException("Thời gian dự kiến thi công không hợp lệ.");

        NoiDung = noiDung;
        TenDonViThiCong = tenDonViThiCong;
        NguoiDaiDien = nguoiDaiDien;
        SoDienThoaiDaiDien = new SoDienThoai(soDienThoaiDaiDien);

        if (TrangThaiThiCongId == TrangThaiThiCong.ChoBoSungHoSo)
        {
            TrangThaiThiCongId = TrangThaiThiCong.ChoDuyetChinhThuc;
        }
    }

    public void DuyetSoBo(int nhanVienId, DateTimeOffset ngayXuLy)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Yêu cầu cần ở trạng thái chờ duyệt mới có thể duyệt sơ bộ.");

        NgayDuyetSoBo = ngayXuLy;
        NguoiXuLyId = nhanVienId;
        TrangThaiThiCongId = TrangThaiThiCong.ChoBoSungHoSo;
    }

    public void DuyetChinhThuc(int nhanVienId, decimal tienDatCoc, DateTimeOffset ngayXuLy)
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.ChoDuyetChinhThuc)
            throw new BusinessException("Cần bổ sung đầy đủ hồ sơ và ở trạng thái chờ duyệt chính thức.");

        TienDatCoc = tienDatCoc;

        // Final approval in base class
        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = nhanVienId;
        NgayXuLy = ngayXuLy;

        TrangThaiThiCongId = TrangThaiThiCong.ChoThuCoc;
    }

    public void XacNhanThuCoc(string? ghiChu)
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.ChoThuCoc)
            throw new BusinessException("Yêu cầu chưa đến bước thu tiền cọc.");

        IsDaThuCoc = true;
        GhiChuThuCoc = ghiChu;
        TrangThaiThiCongId = TrangThaiThiCong.DaCapPhep;

        // Raise event
        AddDomainEvent(new YeuCauThiCongDaCapPhepEvent(this));
    }

    public void BatDauThiCong()
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.DaCapPhep)
            throw new BusinessException("Chỉ có thể bắt đầu thi công khi đã được cấp phép.");

        TrangThaiThiCongId = TrangThaiThiCong.DangThiCong;
    }

    public void HoanTatThiCong()
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.DangThiCong)
            throw new BusinessException("Chỉ có thể hoàn tất khi đang thi công.");

        TrangThaiThiCongId = TrangThaiThiCong.DaHoanTat;
    }

    public void DongYeuCauThiCong()
    {
        if (TrangThaiThiCongId != TrangThaiThiCong.DaHoanTat)
            throw new BusinessException("Chỉ có thể đóng khi đã hoàn tất thi công.");

        TrangThaiThiCongId = TrangThaiThiCong.DaDong;
    }

    public void HuyThiCong(string lyDo)
    {
        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do hủy.");

        LyDo = string.IsNullOrWhiteSpace(LyDo) ? lyDo : $"{LyDo} | {lyDo}";
        TrangThaiThiCongId = TrangThaiThiCong.DaHuy;
    }
}
