using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauSuaChuaAssignedEvent : BaseEvent
{
    public YeuCauSuaChua YeuCauSuaChua { get; }

    public YeuCauSuaChuaAssignedEvent(YeuCauSuaChua yeuCauSuaChua)
    {
        YeuCauSuaChua = yeuCauSuaChua;
    }
}
