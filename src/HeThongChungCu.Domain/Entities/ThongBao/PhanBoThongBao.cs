using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class PhanBoThongBao : BaseEntity
{
    public int ThongBaoId { get; private set; }
    public int NguoiDungId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    private PhanBoThongBao() { } // EF Core

    public PhanBoThongBao(int thongBaoId, int nguoiDungId)
    {
        ThongBaoId = thongBaoId;
        NguoiDungId = nguoiDungId;
        IsRead = false;
    }

    public void MarkAsRead(DateTimeOffset readAt)
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = readAt;
        }
    }
}
