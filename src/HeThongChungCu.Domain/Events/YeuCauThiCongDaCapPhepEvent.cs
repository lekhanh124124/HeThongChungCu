using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Events;

public class YeuCauThiCongDaCapPhepEvent : BaseEvent
{
    public YeuCauThiCongNoiThat YeuCauThiCong { get; }

    public YeuCauThiCongDaCapPhepEvent(YeuCauThiCongNoiThat yeuCauThiCong)
    {
        YeuCauThiCong = yeuCauThiCong;
    }
}
