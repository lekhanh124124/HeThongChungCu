using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauSuaChuaRejectedEvent : BaseEvent
{
    public YeuCauSuaChua YeuCauSuaChua { get; }

    public YeuCauSuaChuaRejectedEvent(YeuCauSuaChua yeuCauSuaChua)
    {
        YeuCauSuaChua = yeuCauSuaChua;
    }
}
