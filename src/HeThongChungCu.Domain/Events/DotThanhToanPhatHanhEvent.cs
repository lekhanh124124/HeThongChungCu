using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class DotThanhToanPhatHanhEvent : BaseEvent
{
    public DotThanhToan DotThanhToan { get; }
    public IEnumerable<HoaDon> HoaDons { get; }

    public DotThanhToanPhatHanhEvent(DotThanhToan dotThanhToan, IEnumerable<HoaDon> hoaDons)
    {
        DotThanhToan = dotThanhToan;
        HoaDons = hoaDons;
    }
}
