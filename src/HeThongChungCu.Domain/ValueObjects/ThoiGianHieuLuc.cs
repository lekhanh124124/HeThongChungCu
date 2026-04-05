using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.ValueObjects;

public record ThoiGianHieuLuc
{
    public DateTimeOffset NgayBatDau { get; private set; }
    public DateTimeOffset? NgayKetThuc { get; private set; }

    public ThoiGianHieuLuc(DateTimeOffset ngayBatDau, DateTimeOffset? ngayKetThuc = null)
    {
        if (ngayKetThuc.HasValue && ngayKetThuc.Value < ngayBatDau)
        {
            throw new BusinessException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
        }

        NgayBatDau = ngayBatDau;
        NgayKetThuc = ngayKetThuc;
    }

    public bool IsActive(DateTimeOffset atDate)
    {
        return atDate >= NgayBatDau && (NgayKetThuc == null || atDate <= NgayKetThuc);
    }

    public bool Overlaps(ThoiGianHieuLuc other)
    {
        var thisEnd = NgayKetThuc ?? DateTimeOffset.MaxValue;
        var otherEnd = other.NgayKetThuc ?? DateTimeOffset.MaxValue;

        return NgayBatDau <= otherEnd && other.NgayBatDau <= thisEnd;
    }
}
