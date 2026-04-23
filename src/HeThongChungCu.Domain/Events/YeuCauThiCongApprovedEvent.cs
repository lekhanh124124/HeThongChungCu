using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongApprovedEvent : BaseEvent
{
    public YeuCauThiCong YeuCauThiCong { get; }

    public YeuCauThiCongApprovedEvent(YeuCauThiCong yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
