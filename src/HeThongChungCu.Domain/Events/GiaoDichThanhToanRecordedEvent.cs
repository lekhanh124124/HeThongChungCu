using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class GiaoDichThanhToanRecordedEvent : BaseEvent
{
    public GiaoDichThanhToan GiaoDichThanhToan { get; }

    public GiaoDichThanhToanRecordedEvent(GiaoDichThanhToan giaoDichThanhToan)
    {
        GiaoDichThanhToan = giaoDichThanhToan;
    }
}
