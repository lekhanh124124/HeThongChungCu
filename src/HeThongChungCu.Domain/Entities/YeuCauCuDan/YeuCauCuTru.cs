using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }
    public CanHo CanHo { get; private set; } = null!;

    // Removed direct UserId navigate for relation changes
    public int? QuanHeCuTruId { get; private set; }
    public QuanHeCuTru? QuanHeCuTru { get; private set; }

    public LoaiYeuCau LoaiYeuCauId { get; private set; } = null!;

    public TrangThaiYeuCau TrangThaiId { get; private set; } = null!;
    public string? LyDo { get; private set; }
    public string? NoiDung { get; private set; }

    public int? NguoiXuLyId { get; private set; }
    public DateTime? NgayXuLy { get; private set; }

    // Proposed changes to User Info (used for 'Them' or 'Sua')
    public string? YeuCauTen { get; private set; }
    public string? YeuCauHo { get; private set; }
    public DateTime? YeuCauNgaySinh { get; private set; }
    public int? YeuCauGioiTinhId { get; private set; }
    public string? YeuCauSoDienThoai { get; private set; }

    // Proposed changes to Relation Info
    public int? YeuCauLoaiQuanHeId { get; private set; }

    private readonly List<YeuCauTaiLieuCuTru> _documents = [];
    public IReadOnlyCollection<YeuCauTaiLieuCuTru> Documents => _documents.AsReadOnly();

    private YeuCauCuTru() { } // EF Core

    private YeuCauCuTru(int canHoId, LoaiYeuCau loaiYeuCau, string? noiDung = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauId = loaiYeuCau;
        NoiDung = noiDung;
        TrangThaiId = TrangThaiYeuCau.Pending;
    }

    public static YeuCauCuTru CreateAddMemberRequest(
        int canHoId, 
        int requesterAccountId, 
        int? quanHeCuTruId, 
        int loaiQuanHeId, 
        string? firstName,
        string? lastName,
        DateTime? dob,
        int? gioiTinhId,
        string? phoneNumber,
        string? noiDung, 
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        DateTimeOffset createdAt)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Them, noiDung)
        {
            QuanHeCuTruId = quanHeCuTruId,
            YeuCauLoaiQuanHeId = loaiQuanHeId,
            YeuCauTen = firstName,
            YeuCauHo = lastName,
            YeuCauNgaySinh = dob,
            YeuCauGioiTinhId = gioiTinhId,
            YeuCauSoDienThoai = phoneNumber
        };

        if (documents != null)
        {
            foreach (var doc in documents)
            {
                request._documents.Add(doc);
            }
        }

        request.SetCreated(requesterAccountId, createdAt);
        return request;
    }

    public static YeuCauCuTru CreateUpdateMemberRequest(
        int canHoId,
        int requesterAccountId,
        int quanHeCuTruId,
        int? newLoaiQuanHeId,
        string? firstName,
        string? lastName,
        DateTime? dob,
        int? gioiTinhId,
        string? phoneNumber,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        DateTimeOffset createdAt)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Sua, noiDung)
        {
            QuanHeCuTruId = quanHeCuTruId,
            YeuCauLoaiQuanHeId = newLoaiQuanHeId,
            YeuCauTen = firstName,
            YeuCauHo = lastName,
            YeuCauNgaySinh = dob,
            YeuCauGioiTinhId = gioiTinhId,
            YeuCauSoDienThoai = phoneNumber
        };

        if (documents != null)
        {
            foreach (var doc in documents)
            {
                request._documents.Add(doc);
            }
        }

        request.SetCreated(requesterAccountId, createdAt);
        return request;
    }

    public static YeuCauCuTru CreateRemoveMemberRequest(
        int canHoId,
        int requesterAccountId,
        int quanHeCuTruId,
        string? noiDung,
        DateTimeOffset createdAt)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Xoa, noiDung)
        {
            QuanHeCuTruId = quanHeCuTruId
        };
        request.SetCreated(requesterAccountId, createdAt);
        return request;
    }

    public void Approve(int adminId, DateTime processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public void Reject(int adminId, string lyDo, DateTime processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể từ chối yêu cầu đang ở trạng thái chờ.");

        if (string.IsNullOrWhiteSpace(lyDo))
            throw new BusinessException("Cần cung cấp lý do từ chối.");

        TrangThaiId = TrangThaiYeuCau.Rejected;
        LyDo = lyDo;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }
}
