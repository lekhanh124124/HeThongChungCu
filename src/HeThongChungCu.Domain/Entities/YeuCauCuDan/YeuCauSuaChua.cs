using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauSuaChua : YeuCau
{
    public PhamViSuaChua PhamViId { get; private set; } = null!;
    public LoaiSuCoKyThuat LoaiSuCoId { get; private set; } = null!;
    public int? HopDongDoiTacId { get; private set; }
    public TrangThaiSuaChua? TrangThaiSuaChuaId { get; private set; }

    private readonly List<NhanSuSuaChua> _nhanSuSuaChuas = [];
    public IReadOnlyCollection<NhanSuSuaChua> NhanSuSuaChuas => _nhanSuSuaChuas.AsReadOnly();

    public string? MoTaViTri { get; private set; }

    public DateTimeOffset? HenTu { get; private set; }
    public DateTimeOffset? HenDen { get; private set; }

    public string? KetQuaXuLy { get; private set; }
    public string? LyDoHuy { get; private set; }

    public decimal? ChiPhiDuKien { get; private set; }
    public decimal? ChiPhiThucTe { get; private set; }
    public bool? IsMienPhi { get; private set; }
    public string? GhiChuBaoGia { get; private set; }
    public int? HoaDonId { get; private set; }

    public void MarkAsBilled(int hoaDonId)
    {
        HoaDonId = hoaDonId;
    }

    private readonly List<TepYeuCauSuaChua> _tepYeuCauSuaChuas = [];
    public IReadOnlyCollection<TepYeuCauSuaChua> TepYeuCauSuaChuas => _tepYeuCauSuaChuas.AsReadOnly();

    private YeuCauSuaChua() : base() { } // EF Core

    private YeuCauSuaChua(
        int canHoId,
        PhamViSuaChua phamVi,
        LoaiSuCoKyThuat loaiSuCo,
        string? noiDung,
        string? moTaViTri,
        TrangThaiYeuCau initialStatus)
        : base(canHoId, LoaiYeuCauCuDan.SuaChua, noiDung, initialStatus)
    {
        PhamViId = phamVi;
        LoaiSuCoId = loaiSuCo;
        MoTaViTri = moTaViTri;
        // TrangThaiSuaChuaId = null (chưa điều phối)
    }

    public static YeuCauSuaChua Create(
        int canHoId,
        PhamViSuaChua phamVi,
        LoaiSuCoKyThuat loaiSuCo,
        string? noiDung,
        string? moTaViTri,
        IEnumerable<TepYeuCauSuaChua>? danhSachTep = null,
        TrangThaiYeuCau? initialStatus = null)
    {
        var status = initialStatus ?? TrangThaiYeuCau.Pending;
        var request = new YeuCauSuaChua(canHoId, phamVi, loaiSuCo, noiDung, moTaViTri, status);

        if (danhSachTep != null)
        {
            foreach (var file in danhSachTep)
            {
                file.MarkAsUsed();
                request._tepYeuCauSuaChuas.Add(file);
            }
        }

        if (status == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauSuaChuaCreatedEvent(request));
        }

        return request;
    }

    /// <summary>
    /// Cập nhật thông tin khi yêu cầu đang ở trạng thái Nháp (Saved).
    /// Nếu danhSachTep != null, toàn bộ tệp cũ sẽ bị thay thế.
    /// </summary>
    public void Update(
        PhamViSuaChua? phamVi,
        LoaiSuCoKyThuat? loaiSuCo,
        string? noiDung,
        string? moTaViTri,
        IEnumerable<TepYeuCauSuaChua>? danhSachTep = null)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Returned)
            throw new BusinessException("Chỉ có thể chỉnh sửa yêu cầu đang ở trạng thái nháp hoặc yêu cầu bổ sung.");

        if (phamVi != null) PhamViId = phamVi;
        if (loaiSuCo != null) LoaiSuCoId = loaiSuCo;
        if (noiDung != null) NoiDung = noiDung;
        if (moTaViTri != null) MoTaViTri = moTaViTri;

        if (danhSachTep != null)
        {
            _tepYeuCauSuaChuas.Clear();
            foreach (var file in danhSachTep)
            {
                file.MarkAsUsed();
                _tepYeuCauSuaChuas.Add(file);
            }
        }
    }

    public override Result Submit()
    {
        var result = base.Submit();
        if (result.IsFailure) return result;

        AddDomainEvent(new YeuCauSuaChuaCreatedEvent(this));

        return Result.Success();
    }

    public override Result Approve(int adminId, DateTimeOffset processedAt)
    {
        var result = base.Approve(adminId, processedAt);
        if (result.IsFailure) return result;

        AddDomainEvent(new YeuCauSuaChuaApprovedEvent(this));

        return Result.Success();
    }

    public override Result Return(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        var result = base.Return(adminId, lyDo, processedAt);
        if (result.IsFailure) return result;

        AddDomainEvent(new YeuCauSuaChuaReturnedEvent(this));

        return Result.Success();
    }

    public override void Reject(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        base.Reject(adminId, lyDo, processedAt);
        AddDomainEvent(new YeuCauSuaChuaRejectedEvent(this));
    }




    public void AssignInternalStaff(IEnumerable<int> nhanVienIds)
    {
        if (TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể giao việc cho yêu cầu đã được duyệt.");

        var ids = nhanVienIds.ToList();
        if (ids.Count == 0)
            throw new BusinessException("Cần chọn ít nhất một nhân viên kỹ thuật.");

        // Clear existing assignments if re-assigning
        HopDongDoiTacId = null;
        _nhanSuSuaChuas.Clear();

        // Thêm bản ghi nhân sự nội bộ cho từng KTV được chọn
        foreach (var id in ids)
        {
            var staff = NhanSuSuaChua.Create(string.Empty, string.Empty, null, "Kỹ thuật viên nội bộ", null, id);
            _nhanSuSuaChuas.Add(staff);
        }

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDieuPhoi;

        AddDomainEvent(new YeuCauSuaChuaAssignedEvent(this));
    }

    /// <summary>
    /// Bổ sung thêm nhân viên kỹ thuật nội bộ sau khi đã điều phối.
    /// Không xóa các nhân sự hiện tại, chỉ thêm mới.
    /// </summary>
    public void AddNhanSuNoiBo(int nhanVienId)
    {
        if (HopDongDoiTacId != null)
            throw new BusinessException("Yêu cầu này đang được xử lý bởi đối tác, không thể bổ sung nhân sự nội bộ.");

        if (TrangThaiId == TrangThaiYeuCau.Completed || TrangThaiId == TrangThaiYeuCau.Cancelled)
            throw new BusinessException("Không thể bổ sung nhân sự cho yêu cầu đã kết thúc.");

        var staff = NhanSuSuaChua.Create(string.Empty, string.Empty, null, "Kỹ thuật viên nội bộ", null, nhanVienId);
        _nhanSuSuaChuas.Add(staff);
    }

    /// <summary>
    /// BQL nhập báo giá sau khi đã liên hệ và xác nhận với cư dân.
    /// Báo giá được chốt trực tiếp - luôn chuyển sang DaDuyetBaoGia.
    /// Thông tin xác nhận từ cư dân được ghi vào GhiChuBaoGia.
    /// </summary>
    public void NhapBaoGia(decimal chiPhi, bool isMienPhi, string? ghiChu)
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DaDieuPhoi && TrangThaiSuaChuaId != TrangThaiSuaChua.ChoBaoGia)
            throw new BusinessException("Chỉ có thể nhập báo giá khi đã được điều phối hoặc đang chờ báo giá.");

        ChiPhiDuKien = chiPhi;
        IsMienPhi = isMienPhi;
        GhiChuBaoGia = ghiChu;

        // BQL chốt báo giá sau khi đã xác nhận trực tiếp với cư dân.
        // Không cần trạng thái chờ duyệt online.
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDuyetBaoGia;

        AddDomainEvent(new YeuCauSuaChuaBaoGiaEvent(this));
    }

    public void AssignPartner(int hopDongId)
    {
        if (TrangThaiId != TrangThaiYeuCau.Approved)
            throw new BusinessException("Chỉ có thể giao việc cho yêu cầu đã được duyệt.");

        HopDongDoiTacId = hopDongId;
        _nhanSuSuaChuas.Clear();

        TrangThaiSuaChuaId = TrangThaiSuaChua.DaDieuPhoi;

        AddDomainEvent(new YeuCauSuaChuaAssignedEvent(this));
    }

    public void AddNhanSuPartner(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null)
    {
        if (HopDongDoiTacId == null)
            throw new BusinessException("Cần gán hợp đồng đối tác trước khi đăng ký nhân sự.");

        if (TrangThaiId == TrangThaiYeuCau.Completed || TrangThaiId == TrangThaiYeuCau.Cancelled)
            throw new BusinessException("Không thể bổ sung nhân sự cho yêu cầu đã kết thúc.");

        var staff = NhanSuSuaChua.Create(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu);
        _nhanSuSuaChuas.Add(staff);
    }

    public void RemoveNhanSu(int nhanSuId, string lyDo)
    {
        if (TrangThaiId == TrangThaiYeuCau.Completed || TrangThaiId == TrangThaiYeuCau.Cancelled)
            throw new BusinessException("Không thể xóa nhân sự cho yêu cầu đã kết thúc.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do xóa nhân sự.");

        var staff = _nhanSuSuaChuas.FirstOrDefault(x => x.Id == nhanSuId && !x.IsDeleted);
        if (staff == null)
            throw new BusinessException("Không tìm thấy thông tin nhân sự đang hoạt động để xóa.");

        // Guard: Nếu yêu cầu đã điều phối/hẹn lịch/báo giá, không được để trống nhân sự
        if (TrangThaiSuaChuaId != null)
        {
            var activeStaffCount = _nhanSuSuaChuas.Count(x => !x.IsDeleted);
            if (activeStaffCount <= 1)
                throw new BusinessException("Yêu cầu đã được điều phối, không thể xóa nhân sự duy nhất. Vui lòng bổ sung nhân sự mới hoặc điều phối lại trước khi xóa.");
        }

        staff.SetReasonForRemoval(lyDo);
        _nhanSuSuaChuas.Remove(staff);
    }

    public void HenLich(DateTimeOffset tuNgay, DateTimeOffset denNgay)
    {
        if (denNgay <= tuNgay)
            throw new BusinessException("Khoảng thời gian hẹn lịch không hợp lệ.");

        HenTu = tuNgay;
        HenDen = denNgay;
        TrangThaiSuaChuaId = TrangThaiSuaChua.DaHenLich;

        AddDomainEvent(new YeuCauSuaChuaHenLichEvent(this));
    }

    /// <summary>
    /// Hoàn tất xử lý yêu cầu. Có thể gọi trực tiếp từ DaDuyetBaoGia hoặc DaHenLich.
    /// Không cần qua bước BatDauXuLy vì không có giao diện cho nhân sự tại hiện trường.
    /// Terminal state: chuyển TrangThaiId sang Completed.
    /// </summary>
    public void HoanTatXuLy(int adminId, string ketQua, decimal? chiPhiThucTe, DateTimeOffset ngayHoanThanh)
    {
        if (TrangThaiSuaChuaId != TrangThaiSuaChua.DaDuyetBaoGia && TrangThaiSuaChuaId != TrangThaiSuaChua.DaHenLich)
            throw new BusinessException("Chỉ có thể hoàn tất khi đã duyệt báo giá hoặc đã hẹn lịch.");

        if (string.IsNullOrWhiteSpace(ketQua))
            throw new BusinessException("Cần cung cấp kết quả xử lý.");

        KetQuaXuLy = ketQua;
        ChiPhiThucTe = chiPhiThucTe;
        NguoiXuLyId = adminId;
        NgayXuLy = ngayHoanThanh;

        // Terminal: chuyển sang Completed ở TrangThaiYeuCau, clear sub-state
        TrangThaiId = TrangThaiYeuCau.Completed;
        TrangThaiSuaChuaId = null;

        // Raise Domain Event for Invoicing
        AddDomainEvent(new YeuCauSuaChuaHoanTatEvent(this));
    }

    public override Result Cancel(int adminId, string lyDo, DateTimeOffset processedAt)
    {
        var result = base.Cancel(adminId, lyDo, processedAt);
        if (result.IsFailure) return result;

        LyDoHuy = lyDo;
        TrangThaiSuaChuaId = null;

        AddDomainEvent(new YeuCauSuaChuaCancelledEvent(this));

        return Result.Success();
    }
}
