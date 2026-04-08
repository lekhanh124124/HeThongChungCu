using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Events;

public class DoiTacHopDongHetHanEvent : BaseEvent
{
    public int DoiTacId { get; }

    public DoiTacHopDongHetHanEvent(int doiTacId)
    {
        DoiTacId = doiTacId;
    }
}
