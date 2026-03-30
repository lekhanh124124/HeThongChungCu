using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }

    // Removed direct navigate for consistency
    public int? QuanHeCuTruId { get; private set; }

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

    private readonly List<YeuCauTaiLieuCuTru> _yeuCauTaiLieuCuTrus = [];
    public IReadOnlyCollection<YeuCauTaiLieuCuTru> YeuCauTaiLieuCuTrus => _yeuCauTaiLieuCuTrus.AsReadOnly();

    private YeuCauCuTru() { } // EF Core

    private YeuCauCuTru(int canHoId, LoaiYeuCau loaiYeuCau, string? noiDung = null, TrangThaiYeuCau? initialStatus = null)
    {
        CanHoId = canHoId;
        LoaiYeuCauId = loaiYeuCau;
        NoiDung = noiDung;
        TrangThaiId = initialStatus ?? TrangThaiYeuCau.Pending;
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
        DateTimeOffset createdAt,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Them, noiDung, initialStatus)
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
                request._yeuCauTaiLieuCuTrus.Add(doc);
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
        DateTimeOffset createdAt,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Sua, noiDung, initialStatus)
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
                request._yeuCauTaiLieuCuTrus.Add(doc);
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
        DateTimeOffset createdAt,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Xoa, noiDung, initialStatus)
        {
            QuanHeCuTruId = quanHeCuTruId
        };
        request.SetCreated(requesterAccountId, createdAt);
        return request;
    }

    public void Approve(int adminId, DateTime processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public void Reject(int adminId, string lyDo, DateTime processedAt)
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
        string? firstName,
        string? lastName,
        DateTime? dob,
        int? gioiTinhId,
        string? phoneNumber,
        int? loaiQuanHeId,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        int modifierAccountId,
        DateTimeOffset updatedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể chỉnh sửa yêu cầu đang ở trạng thái đã lưu.");

        YeuCauTen = firstName;
        YeuCauHo = lastName;
        YeuCauNgaySinh = dob;
        YeuCauGioiTinhId = gioiTinhId;
        YeuCauSoDienThoai = phoneNumber;
        YeuCauLoaiQuanHeId = loaiQuanHeId;
        NoiDung = noiDung;

        if (documents != null)
        {
            _yeuCauTaiLieuCuTrus.Clear();
            foreach (var doc in documents)
            {
                _yeuCauTaiLieuCuTrus.Add(doc);
            }
        }

        SetModified(modifierAccountId, updatedAt);
    }

    public void Submit(int modifierAccountId, DateTimeOffset updatedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể gửi yêu cầu đang ở trạng thái đã lưu.");

        TrangThaiId = TrangThaiYeuCau.Pending;
        SetModified(modifierAccountId, updatedAt);
    }

    public void Withdraw(int modifierAccountId, DateTimeOffset updatedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể thu hồi yêu cầu đang ở trạng thái đã lưu.");

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
        SetModified(modifierAccountId, updatedAt);
    }
}
