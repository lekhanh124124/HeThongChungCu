using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Events;

public class KetThucCuTruEvent : BaseEvent
{
    public int CanHoId { get; }
    public int NguoiDungId { get; }
    public LoaiQuanHeCuTru LoaiQuanHe { get; }

    public KetThucCuTruEvent(int canHoId, int nguoiDungId, LoaiQuanHeCuTru loaiQuanHe)
    {
        CanHoId = canHoId;
        NguoiDungId = nguoiDungId;
        LoaiQuanHe = loaiQuanHe;
    }
}
