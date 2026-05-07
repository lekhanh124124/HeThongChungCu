using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class PhieuBaoTri : AggregateRoot
{
    public string MaPhieu { get; private set; } = null!;
    public int ThietBiId { get; private set; }
    public int HangMucBaoTriId { get; private set; }
    public int? LichBaoTriId { get; private set; }
    public int? HopDongDoiTacId { get; private set; }

    public DateTimeOffset NgayLapPhieu { get; private set; }
    public DateTimeOffset NgayDuKien { get; private set; }
    public DateTimeOffset? NgayThucTe { get; private set; }
    public decimal? ChiPhiThucTe { get; private set; }
    public TrangThaiPhieuBaoTri TrangThaiPhieuBaoTriId { get; private set; } = null!;
    public string? GhiChuXuLy { get; private set; }
    public string? LyDoHuy { get; private set; }
    public int? NguoiKiemDuyetId { get; private set; }

    private readonly List<PhieuBaoTriChecklist> _checklists = [];
    public IReadOnlyCollection<PhieuBaoTriChecklist> Checklists => _checklists.AsReadOnly();

    private readonly List<PhieuBaoTriVatTu> _vatTus = [];
    public IReadOnlyCollection<PhieuBaoTriVatTu> VatTus => _vatTus.AsReadOnly();

    private readonly List<NhanSuBaoTri> _nhanSuBaoTris = [];
    public IReadOnlyCollection<NhanSuBaoTri> NhanSuBaoTris => _nhanSuBaoTris.AsReadOnly();

    private PhieuBaoTri() : base() { } // EF Core

    private PhieuBaoTri(
        string maPhieu,
        int thietBiId,
        int hangMucBaoTriId,
        int? lichBaoTriId,
        DateTimeOffset ngayLapPhieu,
        DateTimeOffset ngayDuKien) : base()
    {
        MaPhieu = maPhieu;
        ThietBiId = thietBiId;
        HangMucBaoTriId = hangMucBaoTriId;
        LichBaoTriId = lichBaoTriId;
        NgayLapPhieu = ngayLapPhieu;
        NgayDuKien = ngayDuKien;
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.ChoGiaoViec;
    }

    public static PhieuBaoTri Create(
        string maPhieu,
        int thietBiId,
        int hangMucBaoTriId,
        int? lichBaoTriId,
        DateTimeOffset ngayLapPhieu,
        DateTimeOffset ngayDuKien,
        IEnumerable<string> standardChecklistItems)
    {
        var phieu = new PhieuBaoTri(
            maPhieu,
            thietBiId,
            hangMucBaoTriId,
            lichBaoTriId,
            ngayLapPhieu,
            ngayDuKien);

        foreach (var item in standardChecklistItems)
        {
            phieu._checklists.Add(PhieuBaoTriChecklist.Create(item));
        }

        return phieu;
    }

    public void AssignStaff(IEnumerable<NhanSuBaoTri> staffs)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.ChoGiaoViec && TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.DaGiaoViec)
            throw new BusinessException("Chỉ có thể giao việc cho phiếu đang chờ hoặc đã giao.");
 
        var staffList = staffs.ToList();
        if (staffList.Count == 0 && HopDongDoiTacId == null)
            throw new BusinessException("Phải phân công ít nhất một kỹ thuật viên hoặc chỉ định đối tác thầu hợp đồng.");
 
        _nhanSuBaoTris.Clear();
        foreach (var staff in staffList)
        {
            _nhanSuBaoTris.Add(staff);
        }
 
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DaGiaoViec;
    }

    public void AssignPartner(int hopDongId)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.ChoGiaoViec && TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.DaGiaoViec)
            throw new BusinessException("Chỉ có thể giao việc cho phiếu đang chờ hoặc đã giao.");
 
        HopDongDoiTacId = hopDongId;
        _nhanSuBaoTris.Clear(); // Trống nếu hoàn toàn giao cho đối tác, hoặc có giám sát nội bộ kèm theo
 
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DaGiaoViec;
    }
 
    public void Start()
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.DaGiaoViec)
            throw new BusinessException("Chỉ có thể bắt đầu khi phiếu đã được giao việc.");
 
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DangThucHien;
    }

    public void UpdateNgayDuKien(DateTimeOffset ngayDuKien)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.ChoGiaoViec && TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.DaGiaoViec)
            throw new BusinessException("Chỉ có thể cập nhật ngày dự kiến khi phiếu chưa bắt đầu bảo trì.");

        NgayDuKien = ngayDuKien;
    }

    public void SubmitResults(
        Dictionary<int, (bool DatYeuCau, string? GhiChu, int? AnhId)> checklistUpdates,
        IEnumerable<PhieuBaoTriVatTu> materials,
        decimal? actualCost,
        string? notes)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.DangThucHien)
            throw new BusinessException("Chỉ có thể nộp kết quả khi đang thực hiện bảo trì.");

        // Cập nhật kết quả checklist
        foreach (var checklist in _checklists)
        {
            if (checklistUpdates.TryGetValue(checklist.Id, out var update))
            {
                checklist.UpdateResult(update.DatYeuCau, update.GhiChu, update.AnhId);
            }
        }

        // Cập nhật vật tư phụ tùng tiêu thụ
        _vatTus.Clear();
        decimal calculatedMaterialCost = 0;
        foreach (var material in materials)
        {
            _vatTus.Add(material);
            calculatedMaterialCost += material.ThanhTien;
        }

        ChiPhiThucTe = (actualCost ?? 0) + calculatedMaterialCost;
        GhiChuXuLy = notes;
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.ChoNghiemThu;
    }
 
    public void NghiemThu(int supervisorId, DateTimeOffset approvalDate)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.ChoNghiemThu)
            throw new BusinessException("Chỉ có thể nghiệm thu phiếu đang chờ nghiệm thu.");
 
        NguoiKiemDuyetId = supervisorId;
        NgayThucTe = approvalDate;
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DaHoanThanh;
    }
 
    public void TuChoi(string reason)
    {
        if (TrangThaiPhieuBaoTriId != TrangThaiPhieuBaoTri.ChoNghiemThu)
            throw new BusinessException("Chỉ có thể từ chối phiếu đang chờ nghiệm thu.");
 
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Cần cung cấp lý do từ chối nghiệm thu.");
 
        GhiChuXuLy = $"[Từ chối nghiệm thu: {reason}] {GhiChuXuLy}";
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DangThucHien;
    }
 
    public void Cancel(string reason)
    {
        if (TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaHoanThanh || TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaHuy)
            throw new BusinessException("Không thể hủy phiếu đã hoàn thành hoặc đã hủy.");
 
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Phải cung cấp lý do hủy phiếu.");
 
        LyDoHuy = reason;
        TrangThaiPhieuBaoTriId = TrangThaiPhieuBaoTri.DaHuy;
    }
}
