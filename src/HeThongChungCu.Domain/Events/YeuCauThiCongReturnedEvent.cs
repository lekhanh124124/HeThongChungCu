using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongReturnedEvent : BaseEvent
{
    public YeuCauThiCong YeuCauThiCong { get; }

    public YeuCauThiCongReturnedEvent(YeuCauThiCong yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
