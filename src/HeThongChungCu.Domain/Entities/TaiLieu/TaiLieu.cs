using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public abstract class TaiLieu : AuditableEntity
{
    public LoaiTaiLieu LoaiTaiLieuId { get; protected set; } = null!;
    public LoaiGiayTo LoaiGiayToId { get; protected set; } = null!;
    public string SoGiayTo { get; protected set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; protected set; }

    protected TaiLieu() { }

    protected TaiLieu(LoaiTaiLieu loaiTaiLieuId, LoaiGiayTo loaiGiayToId, string soGiayTo, DateTimeOffset? ngayPhatHanh)
    {
        LoaiTaiLieuId = loaiTaiLieuId;
        LoaiGiayToId = loaiGiayToId;
        SoGiayTo = soGiayTo;
        NgayPhatHanh = ngayPhatHanh;
    }
}
