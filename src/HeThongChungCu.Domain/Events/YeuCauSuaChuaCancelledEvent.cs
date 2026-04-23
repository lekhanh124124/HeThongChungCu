using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauSuaChuaCancelledEvent : BaseEvent
{
    public YeuCauSuaChua YeuCauSuaChua { get; }

    public YeuCauSuaChuaCancelledEvent(YeuCauSuaChua yeuCauSuaChua)
    {
        YeuCauSuaChua = yeuCauSuaChua;
    }
}
