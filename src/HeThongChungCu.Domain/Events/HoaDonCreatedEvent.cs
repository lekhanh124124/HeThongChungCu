using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ThanhToan.Events;

public class HoaDonCreatedEvent : BaseEvent
{
    public HoaDon HoaDon { get; set; }

    public HoaDonCreatedEvent(HoaDon hoaDon)
    {
        HoaDon = hoaDon;
    }
}
