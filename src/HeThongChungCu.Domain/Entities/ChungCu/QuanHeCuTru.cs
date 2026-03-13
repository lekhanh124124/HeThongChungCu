using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class QuanHeCuTru : AuditableEntity
{
    public int CanHoId { get; private set; }
    public int UserId { get; private set; }
    public LoaiQuanHeCuTru LoaiQuanHeCuTruId { get; private set; } = null!;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool IsKetThuc { get; private set; }

    private QuanHeCuTru() { } // EF Core

    internal QuanHeCuTru(int canHoId, int userId, LoaiQuanHeCuTru loaiQuanHeCuTruId, DateTime ngayBatDau)
    {
        CanHoId = canHoId;
        UserId = userId;
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
        NgayBatDau = ngayBatDau;
        IsKetThuc = false;
    }

    public void ThayDoiLoaiQuanHe(LoaiQuanHeCuTru loaiQuanHeCuTruId)
    {
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
    }

    public void KetThucCuTru(DateTime ngayKetThuc)
    {
        NgayKetThuc = ngayKetThuc;
        IsKetThuc = true;
    }
}
