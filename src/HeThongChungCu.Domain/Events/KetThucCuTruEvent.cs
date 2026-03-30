using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Events;

public class KetThucCuTruEvent : BaseEvent
{
    public int CanHoId { get; }
    public LoaiQuanHeCuTru LoaiQuanHe { get; }

    public KetThucCuTruEvent(int canHoId, LoaiQuanHeCuTru loaiQuanHe)
    {
        CanHoId = canHoId;
        LoaiQuanHe = loaiQuanHe;
    }
}
