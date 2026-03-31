using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauCuTruCreatedEvent : BaseEvent
{
    public YeuCauCuTru YeuCau { get; }

    public YeuCauCuTruCreatedEvent(YeuCauCuTru yeuCau)
    {
        YeuCau = yeuCau;
    }
}
