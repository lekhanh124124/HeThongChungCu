using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongCompletedEvent : BaseEvent
{
    public YeuCauThiCong YeuCauThiCong { get; }

    public YeuCauThiCongCompletedEvent(YeuCauThiCong yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
