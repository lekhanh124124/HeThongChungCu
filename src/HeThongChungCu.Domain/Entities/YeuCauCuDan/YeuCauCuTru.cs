using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauCuTru : YeuCau
{
    // Proposed changes to User Info (used for 'Them' or 'Sua')
    public int? YeuCauQuanHeCuTruId { get; private set; }
    public string? YeuCauTen { get; private set; }
    public string? YeuCauHo { get; private set; }
    public DateTime? YeuCauNgaySinh { get; private set; }
    public int? YeuCauGioiTinhId { get; private set; }
    public string? YeuCauSoDienThoai { get; private set; }
    public string? YeuCauCCCD { get; private set; }
    public string? YeuCauDiaChi { get; private set; }
    public int? YeuCauLoaiQuanHeId { get; private set; }

    private readonly List<YeuCauTaiLieuCuTru> _yeuCauTaiLieuCuTrus = [];
    public IReadOnlyCollection<YeuCauTaiLieuCuTru> YeuCauTaiLieuCuTrus => _yeuCauTaiLieuCuTrus.AsReadOnly();

    private YeuCauCuTru() { } // EF Core

    private YeuCauCuTru(int canHoId, LoaiYeuCau loaiYeuCau, string? noiDung = null, TrangThaiYeuCau? initialStatus = null)
        : base(canHoId, loaiYeuCau, noiDung, initialStatus)
    {
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

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauCuTruCreatedEvent(request));
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

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauCuTruCreatedEvent(request));
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

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauCuTruCreatedEvent(request));
        }

        return request;
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
            foreach (var doc in _yeuCauTaiLieuCuTrus)
            {
                foreach (var file in doc.Files)
                {
                    file.MarkAsUnused();
                }
            }
            _yeuCauTaiLieuCuTrus.Clear();
            foreach (var doc in documents)
            {
                _yeuCauTaiLieuCuTrus.Add(doc);
            }
        }
    }

    public override void Submit()
    {
        base.Submit();
        AddDomainEvent(new YeuCauCuTruCreatedEvent(this));
    }

    public override void Withdraw()
    {
        base.Withdraw();
    }

    public void Invalidate(string? lyDo)
    {
        if (TrangThaiId != TrangThaiYeuCau.Pending && TrangThaiId != TrangThaiYeuCau.Saved)
            return;

        TrangThaiId = TrangThaiYeuCau.Invalidated;
        LyDo = string.IsNullOrWhiteSpace(LyDo) ? lyDo : $"{LyDo} | {lyDo}";
    }
}
