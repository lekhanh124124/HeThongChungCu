using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class HoaDonDoiTacPaidEvent : BaseEvent
{
    public HoaDonDoiTac HoaDonDoiTac { get; }

    public HoaDonDoiTacPaidEvent(HoaDonDoiTac hoaDonDoiTac)
    {
        HoaDonDoiTac = hoaDonDoiTac;
    }
}
