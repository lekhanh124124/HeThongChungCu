using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongHoanCocEvent : BaseEvent
{
    public YeuCauThiCong YeuCauThiCong { get; }

    public YeuCauThiCongHoanCocEvent(YeuCauThiCong yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
