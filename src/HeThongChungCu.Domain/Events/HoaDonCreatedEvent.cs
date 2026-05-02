using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class HoaDonCreatedEvent : BaseEvent
{
    public HoaDon HoaDon { get; set; }

    public HoaDonCreatedEvent(HoaDon hoaDon)
    {
        HoaDon = hoaDon;
    }
}
