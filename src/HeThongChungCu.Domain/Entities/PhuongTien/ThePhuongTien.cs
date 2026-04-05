using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class ThePhuongTien : AuditableEntity
{
    public int PhuongTienId { get; private set; }
    public string MaThe { get; private set; } = string.Empty;
    public ThoiGianHieuLuc ThoiGian { get; private set; } = null!;
    
    public TrangThaiThePhuongTien TrangThaiId { get; private set; } = default!;

    public bool IsInUse => TrangThaiId == TrangThaiThePhuongTien.Active;

    private ThePhuongTien() { }

    internal ThePhuongTien(int phuongTienId, string maThe, DateTimeOffset ngayBatDau)
    {
        if (string.IsNullOrWhiteSpace(maThe))
            throw new BusinessException("Mã thẻ không được để trống.");

        PhuongTienId = phuongTienId;
        MaThe = maThe;
        ThoiGian = new ThoiGianHieuLuc(ngayBatDau);
        TrangThaiId = TrangThaiThePhuongTien.Active;
    }

    public void KhoaThe(DateTimeOffset ngayKetThuc)
    {
        if (TrangThaiId == TrangThaiThePhuongTien.Locked || TrangThaiId == TrangThaiThePhuongTien.Lost)
            return;

        ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, ngayKetThuc);
        TrangThaiId = TrangThaiThePhuongTien.Locked;
    }

    public void BaoMat(DateTimeOffset now)
    {
        if (TrangThaiId == TrangThaiThePhuongTien.Lost)
            return;

        ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, now);
        TrangThaiId = TrangThaiThePhuongTien.Lost;
    }
}