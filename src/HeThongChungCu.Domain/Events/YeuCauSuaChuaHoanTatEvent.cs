using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauSuaChuaHoanTatEvent : BaseEvent
{
    public YeuCauSuaChua YeuCauSuaChua { get; }

    public YeuCauSuaChuaHoanTatEvent(YeuCauSuaChua yeuCauSuaChua)
    {
        YeuCauSuaChua = yeuCauSuaChua;
    }
}
