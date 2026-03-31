using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauPhuongTienCreatedEvent : BaseEvent
{
    public YeuCauPhuongTien YeuCau { get; }

    public YeuCauPhuongTienCreatedEvent(YeuCauPhuongTien yeuCau)
    {
        YeuCau = yeuCau;
    }
}
