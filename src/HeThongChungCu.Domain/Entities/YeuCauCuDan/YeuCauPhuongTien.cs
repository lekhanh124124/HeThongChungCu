using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauPhuongTien : YeuCau
{
    // Proposed changes to Vehicle Info (used for 'Them' or 'Sua')
    public int? YeuCauPhuongTienId { get; private set; }
    public string YeuCauTenPhuongTien { get; private set; } = string.Empty;
    public LoaiPhuongTien YeuCauLoaiPhuongTienId { get; private set; } = default!;
    public string YeuCauBienSo { get; private set; } = string.Empty;
    public string YeuCauMauXe { get; private set; } = string.Empty;

    private readonly List<TepTaiLieu> _yeuCauHinhAnhPhuongTiens = [];
    public IReadOnlyCollection<TepTaiLieu> YeuCauHinhAnhPhuongTiens => _yeuCauHinhAnhPhuongTiens.AsReadOnly();

    private YeuCauPhuongTien() { } // EF Core

    private YeuCauPhuongTien(
        int canHoId,
        LoaiYeuCau loaiYeuCau,
        TrangThaiYeuCau trangThaiId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung = null,
        int? phuongTienId = null)
        : base(canHoId, loaiYeuCau, noiDung, trangThaiId)
    {
        YeuCauLoaiPhuongTienId = loaiPhuongTien;
        YeuCauTenPhuongTien = tenPhuongTien;
        YeuCauBienSo = bienSo;
        YeuCauMauXe = mauXe;
        YeuCauPhuongTienId = phuongTienId;
    }

    public static YeuCauPhuongTien CreateAddRequest(
        int canHoId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Them, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung);

        if (images != null)
        {
            foreach (var img in images)
            {
                img.MarkAsUsed();
                request._yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }

    public static YeuCauPhuongTien CreateUpdateRequest(
        int canHoId,
        int phuongTienId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Sua, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung, phuongTienId);

        if (images != null)
        {
            foreach (var img in images)
            {
                img.MarkAsUsed();
                request._yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }

    public static YeuCauPhuongTien CreateDeleteRequest(
        int canHoId,
        int phuongTienId,
        LoaiPhuongTien loaiPhuongTien,
        string tenPhuongTien,
        string bienSo,
        string mauXe,
        string? noiDung,
        TrangThaiYeuCau trangThaiId)
    {
        var request = new YeuCauPhuongTien(canHoId, LoaiYeuCau.Xoa, trangThaiId, loaiPhuongTien, tenPhuongTien, bienSo, mauXe, noiDung, phuongTienId);

        if (request.TrangThaiId == TrangThaiYeuCau.Pending)
        {
            request.AddDomainEvent(new YeuCauPhuongTienCreatedEvent(request));
        }

        return request;
    }


    public void Update(
        LoaiPhuongTien? loaiPhuongTien,
        string? tenPhuongTien,
        string? bienSo,
        string? mauXe,
        string? noiDung,
        IEnumerable<TepTaiLieu>? images)
    {
        if (TrangThaiId != TrangThaiYeuCau.Saved)
            throw new BusinessException("Chỉ có thể chỉnh sửa yêu cầu đang ở trạng thái đã lưu.");

        if (loaiPhuongTien != null) YeuCauLoaiPhuongTienId = loaiPhuongTien;
        if (!string.IsNullOrEmpty(tenPhuongTien)) YeuCauTenPhuongTien = tenPhuongTien;
        if (!string.IsNullOrEmpty(bienSo)) YeuCauBienSo = bienSo;
        if (!string.IsNullOrEmpty(mauXe)) YeuCauMauXe = mauXe;
        NoiDung = noiDung;

        if (images != null)
        {
            foreach (var img in _yeuCauHinhAnhPhuongTiens)
            {
                img.MarkAsUnused();
            }
            _yeuCauHinhAnhPhuongTiens.Clear();
            foreach (var img in images)
            {
                img.MarkAsUsed();
                _yeuCauHinhAnhPhuongTiens.Add(img);
            }
        }
    }

    public override void Submit()
    {
        base.Submit();
        AddDomainEvent(new YeuCauPhuongTienCreatedEvent(this));
    }

    public override void Withdraw()
    {
        base.Withdraw();
    }
}
