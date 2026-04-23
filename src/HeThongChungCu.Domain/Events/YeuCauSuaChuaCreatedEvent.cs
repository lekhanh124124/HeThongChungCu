using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauSuaChuaCreatedEvent : BaseEvent
{
    public YeuCauSuaChua YeuCauSuaChua { get; }

    public YeuCauSuaChuaCreatedEvent(YeuCauSuaChua yeuCauSuaChua)
    {
        YeuCauSuaChua = yeuCauSuaChua;
    }
}
