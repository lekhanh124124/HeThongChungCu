using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauPhanAnhCreatedEvent : BaseEvent
{
    public YeuCauPhanAnh YeuCauPhanAnh { get; }

    public YeuCauPhanAnhCreatedEvent(YeuCauPhanAnh yeuCauPhanAnh)
    {
        YeuCauPhanAnh = yeuCauPhanAnh;
    }
}
