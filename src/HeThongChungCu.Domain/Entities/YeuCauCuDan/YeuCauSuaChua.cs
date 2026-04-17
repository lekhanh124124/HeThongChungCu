using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauSuaChua : YeuCau
{
    public PhamViSuaChua PhamViId { get; private set; } = null!;
    public LoaiSuCoKyThuat LoaiSuCoId { get; private set; } = null!;
    public int? HopDongDoiTacId { get; private set; }

    public TrangThaiSuaChua TrangThaiSuaChuaId { get; private set; } = null!;

    private readonly List<NhanSuSuaChua> _nhanSuSuaChuas = [];
    public IReadOnlyCollection<NhanSuSuaChua> NhanSuSuaChuas => _nhanSuSuaChuas.AsReadOnly();

    public MucDoUuTien MucDoUuTienDeXuatId { get; private set; } = null!;
    public MucDoUuTien? MucDoUuTienChotId { get; private set; }
    public int? NguoiChotUuTienId { get; private set; }
    public DateTimeOffset? NgayChotUuTien { get; private set; }

    public string? MoTaViTri { get; private set; }

    public DateTimeOffset? HenTu { get; private set; }
    public DateTimeOffset? HenDen { get; private set; }

    public string? KetQuaXuLy { get; private set; }
    public string? LyDoHuy { get; private set; }

    public decimal? ChiPhiDuKien { get; private set; }
    public decimal? ChiPhiThucTe { get; private set; }
    public bool? IsMienPhi { get; private set; }
    public string? GhiChuBaoGia { get; private set; }

    private readonly List<TepYeuCauSuaChua> _tepYeuCauSuaChuas = [];
    public IReadOnlyCollection<TepYeuCauSuaChua> TepYeuCauSuaChuas => _tepYeuCauSuaChuas.AsReadOnly();

    private YeuCauSuaChua() : base() { } // EF Core

    private YeuCauSuaChua(
        int canHoId,
        PhamViSuaChua phamVi,
        LoaiSuCoKyThuat loaiSuCo,
        MucDoUuTien mucDoUuTienDeXuat,
        string? noiDung,
        string? moTaViTri,
        TrangThaiSuaChua? trangThaiBanDau = null)
        : base(canHoId, LoaiYeuCauCuDan.SuaChua, noiDung, TrangThaiYeuCau.Approved)
    {
        PhamViId = phamVi;
        LoaiSuCoId = loaiSuCo;
        MucDoUuTienDeXuatId = mucDoUuTienDeXuat;
        MoTaViTri = moTaViTri;
        TrangThaiSuaChuaId = trangThaiBanDau ?? TrangThaiSuaChua.MoiTao;
    }

    public static YeuCauSuaChua Create(
        int canHoId,
        PhamViSuaChua phamVi,
        LoaiSuCoKyThuat loaiSuCo,
        MucDoUuTien mucDoUuTienDeXuat,
        string? noiDung,
        string? moTaViTri,
        IEnumerable<TepYeuCauSuaChua>? danhSachTep = null)
    {
        var request = new YeuCauSuaChua(canHoId, phamVi, loaiSuCo, mucDoUuTienDeXuat, noiDung, moTaViTri);

        if (danhSachTep != null)
        {
            foreach (var file in danhSachTep)
            {
                file.MarkAsUsed();
                request._tepYeuCauSuaChuas.Add(file);
            }
        }

        return request;
    }

    public void TiepNhan(int nhanVienId, DateTimeOffset ngayTiepNhan)
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.MoiTao)
            throw new BusinessException("Chỉ có thể tiếp nhận yêu cầu ở trạng thái mới tạo.");

        NguoiXuLyId = nhanVienId;
        NgayXuLy = ngayTiepNhan;
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaTiepNhan;
    }

    public void ChotUuTien(int nhanVienId, MucDoUuTien mucDoUuTienChot, DateTimeOffset ngayXuLy)
    {
        if (TrangThaiSuaChuaId == TrangThaiSuaChua.DaDong || TrangThaiSuaChuaId == TrangThaiSuaChua.DaHuy)
            throw new BusinessException("Không thể chốt ưu tiên cho yêu cầu đã đóng/hủy.");

        MucDoUuTienChotId = mucDoUuTienChot;
        NguoiChotUuTienId = nhanVienId;
        NgayChotUuTien = ngayXuLy;
    }

    public void AssignInternalStaff(int nhanVienId)
    {
        if (TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể giao việc cho yêu cầu đã được duyệt.");

        // Clear existing assignments if switching
        HopDongDoiTacId = null;
        _nhanSuSuaChuas.Clear();

        // Thêm bản ghi nhân sự nội bộ
        var staff = NhanSuSuaChua.Create(string.Empty, string.Empty, null, "Kỹ thuật viên nội bộ", null, nhanVienId);
        _nhanSuSuaChuas.Add(staff);

        if (TrangThaiSuaChuaId == TrangThaiSuaChua.MoiTao)
            TrangThaiSuaChuaId = TrangThaiSuaChua.DaTiepNhan;

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDieuPhoi;
    }

    public void XacNhanKiemTra()
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DaDieuPhoi)
            throw new BusinessException("Cần điều phối nhân sự trước khi xác nhận kiểm tra.");

        TrangThaiSuaChuaId = TrangThaiSuaChua.ChoKiemTra;
    }

    public void NhapBaoGia(decimal chiPhi, bool isMienPhi, string? ghiChu)
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.ChoKiemTra && TrangThaiSuaChuaId != TrangThaiSuaChua.ChoBaoGia)
            throw new BusinessException("Chỉ có thể nhập báo giá khi đang ở trạng thái chờ kiểm tra hoặc chờ báo giá.");

        ChiPhiDuKien = chiPhi;
        IsMienPhi = isMienPhi;
        GhiChuBaoGia = ghiChu;

        if (isMienPhi)
        {
            TrangThaiSuaChuaId = TrangThaiSuaChua.DaDuyetBaoGia;
        }
        else
        {
            TrangThaiSuaChuaId = TrangThaiSuaChua.ChoCuDanDuyetBaoGia;
        }
    }

    public void CuDanDuyetBaoGia()
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.ChoCuDanDuyetBaoGia)
            throw new BusinessException("Hiện tại không có báo giá nào đang chờ bạn duyệt.");

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDuyetBaoGia;
    }

    public void AssignPartner(int hopDongId)
    {
        if (TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể giao việc cho yêu cầu đã được duyệt.");

        HopDongDoiTacId = hopDongId;
        _nhanSuSuaChuas.Clear();

        if (TrangThaiSuaChuaId == TrangThaiSuaChua.MoiTao)
            TrangThaiSuaChuaId = TrangThaiSuaChua.DaTiepNhan;

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDieuPhoi;
    }

    public void AddNhanSuPartner(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null)
    {
        if (HopDongDoiTacId == null)
            throw new BusinessException("Cần gán hợp đồng đối tác trước khi đăng ký nhân sự.");

        if (TrangThaiSuaChuaId == TrangThaiSuaChua.DaDong || TrangThaiSuaChuaId == TrangThaiSuaChua.DaHuy)
            throw new BusinessException("Không thể bổ sung nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        var staff = NhanSuSuaChua.Create(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu);
        _nhanSuSuaChuas.Add(staff);
    }

    public void UpdateNhanSu(int nhanSuId, string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
    {
        if (TrangThaiSuaChuaId == TrangThaiSuaChua.DaDong || TrangThaiSuaChuaId == TrangThaiSuaChua.DaHuy)
            throw new BusinessException("Không thể cập nhật nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        var staff = _nhanSuSuaChuas.FirstOrDefault(x => x.Id == nhanSuId);
        if (staff == null)
            throw new BusinessException("Không tìm thấy thông tin nhân sự để cập nhật.");

        staff.UpdateInfo(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu, nhanVienId);
    }

    public void RemoveNhanSu(int nhanSuId)
    {
        if (TrangThaiSuaChuaId == TrangThaiSuaChua.DaDong || TrangThaiSuaChuaId == TrangThaiSuaChua.DaHuy)
            throw new BusinessException("Không thể xóa nhân sự cho yêu cầu đã đóng hoặc đã hủy.");

        var staff = _nhanSuSuaChuas.FirstOrDefault(x => x.Id == nhanSuId);
        if (staff != null)
        {
            _nhanSuSuaChuas.Remove(staff);
        }
    }

    public void HenLich(DateTimeOffset tuNgay, DateTimeOffset denNgay)
    {
        if (denNgay <= tuNgay)
            throw new BusinessException("Khoảng thời gian hẹn lịch không hợp lệ.");

        HenTu = tuNgay;
        HenDen = denNgay;
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaHenLich;
    }

    public void BatDauXuLy()
    {
        if (!_nhanSuSuaChuas.Any() && HopDongDoiTacId == null)
            throw new BusinessException("Yêu cầu chưa được điều phối nhân sự hoặc đối tác để bắt đầu xử lý.");

        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DaDuyetBaoGia && TrangThaiSuaChuaId != TrangThaiSuaChua.DaHenLich)
            throw new BusinessException("Chỉ có thể bắt đầu xử lý khi đã duyệt báo giá hoặc đã hẹn lịch.");

        TrangThaiSuaChuaId = TrangThaiSuaChua.DangXuLy;
    }

    public void HoanTatXuLy(string ketQua, decimal? chiPhiThucTe, DateTimeOffset ngayHoanThanh)
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DangXuLy)
            throw new BusinessException("Chỉ có thể hoàn tất khi đang xử lý.");

        if (string.IsNullOrWhiteSpace(ketQua))
            throw new BusinessException("Cần cung cấp kết quả xử lý.");

        KetQuaXuLy = ketQua;
        ChiPhiThucTe = chiPhiThucTe;
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaXuLy;
        NgayXuLy = ngayHoanThanh;

        // Raise Domain Event for Invoicing
        AddDomainEvent(new YeuCauSuaChuaHoanTatEvent(this));
    }

    public void DongYeuCau()
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DaXuLy)
            throw new BusinessException("Chỉ có thể đóng yêu cầu khi đã xử lý.");

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDong;
    }

    public void Huy(string lyDo)
    {
        if (TrangThaiSuaChuaId == TrangThaiSuaChua.DaDong)
            throw new BusinessException("Không thể hủy yêu cầu đã đóng.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do hủy.");

        LyDoHuy = lyDo;
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaHuy;
    }
}
