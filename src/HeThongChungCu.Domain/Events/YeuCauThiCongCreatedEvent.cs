using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongCreatedEvent : BaseEvent
{
    public YeuCauThiCong YeuCauThiCong { get; }

    public YeuCauThiCongCreatedEvent(YeuCauThiCong yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
