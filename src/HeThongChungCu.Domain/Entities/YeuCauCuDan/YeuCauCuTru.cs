using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }

    public LoaiYeuCau LoaiYeuCauId { get; private set; } = null!;

    public TrangThaiYeuCau TrangThaiId { get; private set; } = null!;
    public string? LyDo { get; private set; }
    public string? NoiDung { get; private set; }

    public int? NguoiXuLyId { get; private set; }
    public DateTimeOffset? NgayXuLy { get; private set; }

    // Proposed changes to User Info (used for 'Them' or 'Sua')
    public int? YeuCauQuanHeCuTruId { get; private set; }
    public string? YeuCauTen { get; private set; }
    public string? YeuCauHo { get; private set; }
    public DateTime? YeuCauNgaySinh { get; private set; }
    public int? YeuCauGioiTinhId { get; private set; }
    public string? YeuCauSoDienThoai { get; private set; }
    public string? YeuCauCCCD { get; private set; }
    public string? YeuCauDiaChi { get; private set; }

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
        int? quanHeCuTruYeuCauId,
        int loaiQuanHeYeuCauId,
        string? tenYeuCau,
        string? hoYeuCau,
        DateTime? ngaySinhYeuCau,
        int? gioiTinhYeuCauId,
        string? soDienThoaiYeuCau,
        string? cccdYeuCau,
        string? diaChiYeuCau,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Them, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruYeuCauId,
            YeuCauLoaiQuanHeId = loaiQuanHeYeuCauId,
            YeuCauTen = tenYeuCau,
            YeuCauHo = hoYeuCau,
            YeuCauNgaySinh = ngaySinhYeuCau,
            YeuCauGioiTinhId = gioiTinhYeuCauId,
            YeuCauSoDienThoai = soDienThoaiYeuCau,
            YeuCauCCCD = cccdYeuCau,
            YeuCauDiaChi = diaChiYeuCau
        };

        if (documents != null)
        {
            foreach (var doc in documents)
            {
                request._yeuCauTaiLieuCuTrus.Add(doc);
            }
        }

        return request;
    }

    public static YeuCauCuTru CreateUpdateMemberRequest(
        int canHoId,
        int quanHeCuTruId,
        int? newLoaiQuanHeId,
        string? firstName,
        string? lastName,
        DateTime? dob,
        int? gioiTinhId,
        string? phoneNumber,
        string? cccd,
        string? diaChi,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Sua, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruId,
            YeuCauLoaiQuanHeId = newLoaiQuanHeId,
            YeuCauTen = firstName,
            YeuCauHo = lastName,
            YeuCauNgaySinh = dob,
            YeuCauGioiTinhId = gioiTinhId,
            YeuCauSoDienThoai = phoneNumber,
            YeuCauCCCD = cccd,
            YeuCauDiaChi = diaChi
        };

        if (documents != null)
        {
            foreach (var doc in documents)
            {
                request._yeuCauTaiLieuCuTrus.Add(doc);
            }
        }

        return request;
    }

    public static YeuCauCuTru CreateRemoveMemberRequest(
        int canHoId,
        int quanHeCuTruId,
        string? noiDung,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiYeuCau.Xoa, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruId
        };
        return request;
    }

    public void Approve(int adminId, DateTimeOffset processedAt)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể duyệt yêu cầu đang ở trạng thái chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Approved;
        NguoiXuLyId = adminId;
        NgayXuLy = processedAt;
    }

    public void Reject(int adminId, string lyDo, DateTimeOffset processedAt)
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
        string? cccd,
        string? diaChi,
        int? loaiQuanHeId,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể chỉnh sửa yêu cầu đang ở trạng thái đã lưu.");

        YeuCauTen = firstName;
        YeuCauHo = lastName;
        YeuCauNgaySinh = dob;
        YeuCauGioiTinhId = gioiTinhId;
        YeuCauSoDienThoai = phoneNumber;
        YeuCauCCCD = cccd;
        YeuCauDiaChi = diaChi;
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
    }

    public void Submit()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Withdrawn)
            throw new BusinessException("Chỉ có thể gửi yêu cầu đang ở trạng thái đã lưu hoặc đã thu hồi.");

        TrangThaiId = TrangThaiYeuCau.Pending;
    }

    public void Withdraw()
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved && TrangThaiId != TrangThaiYeuCau.Pending)
            throw new BusinessException("Chỉ có thể thu hồi yêu cầu đang ở trạng thái đã lưu hoặc đang chờ duyệt.");

        TrangThaiId = TrangThaiYeuCau.Withdrawn;
    }

    public void Invalidate(string? lyDo)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending && TrangThaiId != TrangThaiYeuCau.Saved)
            return;

        TrangThaiId = TrangThaiYeuCau.Invalidated;
        LyDo = string.IsNullOrWhiteSpace(LyDo) ? lyDo : $"{LyDo} | {lyDo}";
    }
}
