using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class KhaoSatPublishedEvent : BaseEvent
{
    public int KhaoSatId { get; }
    public string TieuDe { get; }

    public KhaoSatPublishedEvent(int khaoSatId, string tieuDe)
    {
        KhaoSatId = khaoSatId;
        TieuDe = tieuDe;
    }
}
