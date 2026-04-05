using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public abstract class TaiLieu : AuditableEntity
{
    public LoaiGiayTo LoaiGiayToId { get; protected set; } = null!;
    public string SoGiayTo { get; protected set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; protected set; }

    protected TaiLieu() { }

    protected TaiLieu(LoaiGiayTo loaiGiayToId, string soGiayTo, DateTimeOffset? ngayPhatHanh)
    {
        LoaiGiayToId = loaiGiayToId;
        SoGiayTo = soGiayTo;
        NgayPhatHanh = ngayPhatHanh;
    }
}
