using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Events;

public class UserRegisteredEvent : BaseEvent
{
    public int NguoiDungId { get; }
    public string TenDangNhap { get; }

    public UserRegisteredEvent(int nguoiDungId, string tenDangNhap)
    {
        NguoiDungId = nguoiDungId;
        TenDangNhap = tenDangNhap;
    }
}
