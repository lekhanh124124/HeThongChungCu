using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauCuTru : YeuCau
{
    public LoaiHanhDongYeuCau LoaiHanhDongYeuCauId { get; private set; } = null!;

    // Proposed changes to User Info (used for 'Them' or 'Sua')
    public int? YeuCauQuanHeCuTruId { get; private set; }
    public string? YeuCauTen { get; private set; }
    public string? YeuCauHo { get; private set; }
    public DateTimeOffset? YeuCauNgaySinh { get; private set; }
    public int? YeuCauGioiTinhId { get; private set; }
    public SoDienThoai? YeuCauSoDienThoai { get; private set; }
    public string? YeuCauCCCD { get; private set; }
    public DiaChi YeuCauDiaChi { get; private set; } = null!;
    public int? YeuCauLoaiQuanHeId { get; private set; }

    private readonly List<YeuCauTaiLieuCuTru> _yeuCauTaiLieuCuTrus = [];
    public IReadOnlyCollection<YeuCauTaiLieuCuTru> YeuCauTaiLieuCuTrus => _yeuCauTaiLieuCuTrus.AsReadOnly();

    private YeuCauCuTru() : base() { } // EF Core

    private YeuCauCuTru(int canHoId, LoaiHanhDongYeuCau loaiYeuCau, string? noiDung = null, TrangThaiYeuCau? initialStatus = null)
        : base(canHoId, LoaiYeuCauCuDan.CuTru, noiDung, initialStatus)
    {
        LoaiHanhDongYeuCauId = loaiYeuCau;
    }

    public static YeuCauCuTru CreateAddMemberRequest(
        int canHoId,
        int? quanHeCuTruYeuCauId,
        int loaiQuanHeYeuCauId,
        string? tenYeuCau,
        string? hoYeuCau,
        DateTimeOffset? ngaySinhYeuCau,
        int? gioiTinhYeuCauId,
        string? soDienThoaiYeuCau,
        string? cccdYeuCau,
        string? diaChiYeuCau,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiHanhDongYeuCau.Them, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruYeuCauId,
            YeuCauLoaiQuanHeId = loaiQuanHeYeuCauId,
            YeuCauTen = tenYeuCau,
            YeuCauHo = hoYeuCau,
            YeuCauNgaySinh = ngaySinhYeuCau,
            YeuCauGioiTinhId = gioiTinhYeuCauId,
            YeuCauSoDienThoai = new SoDienThoai(soDienThoaiYeuCau),
            YeuCauCCCD = cccdYeuCau,
            YeuCauDiaChi = new DiaChi(diaChiYeuCau)
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
        DateTimeOffset? dob,
        int? gioiTinhId,
        string? phoneNumber,
        string? cccd,
        string? diaChi,
        string? noiDung,
        IEnumerable<YeuCauTaiLieuCuTru>? documents,
        TrangThaiYeuCau? initialStatus = null)
    {
        var request = new YeuCauCuTru(canHoId, LoaiHanhDongYeuCau.Sua, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruId,
            YeuCauLoaiQuanHeId = newLoaiQuanHeId,
            YeuCauTen = firstName,
            YeuCauHo = lastName,
            YeuCauNgaySinh = dob,
            YeuCauGioiTinhId = gioiTinhId,
            YeuCauSoDienThoai = new SoDienThoai(phoneNumber),
            YeuCauCCCD = cccd,
            YeuCauDiaChi = new DiaChi(diaChi)
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
        var request = new YeuCauCuTru(canHoId, LoaiHanhDongYeuCau.Xoa, noiDung, initialStatus)
        {
            YeuCauQuanHeCuTruId = quanHeCuTruId,
            YeuCauDiaChi = new DiaChi(null)
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
        DateTimeOffset? dob,
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
        YeuCauSoDienThoai = new SoDienThoai(phoneNumber);
        YeuCauCCCD = cccd;
        YeuCauDiaChi = new DiaChi(diaChi);
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
    public static YeuCauTaiLieuCuTru CreateYeuCauTaiLieuCuTru(
        LoaiGiayTo loaiGiayTo,
        string soGiayTo,
        DateTime? ngayPhatHanh,
        IEnumerable<TepYeuCauTaiLieuCuTru>? files = null,
        int? taiLieuCuTruId = null)
    {
        return new YeuCauTaiLieuCuTru(loaiGiayTo, soGiayTo, ngayPhatHanh, files, taiLieuCuTruId);
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
