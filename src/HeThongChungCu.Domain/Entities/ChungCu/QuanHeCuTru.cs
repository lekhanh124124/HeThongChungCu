using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class QuanHeCuTru : AuditableEntity
{
    public int CanHoId { get; private set; }
    public int UserId { get; private set; }
    public int LoaiQuanHeCuTruId { get; private set; }
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool TrangThai { get; private set; }

    private QuanHeCuTru() { } // EF Core

    internal QuanHeCuTru(int canHoId, int userId, int loaiQuanHeCuTruId, DateTime ngayBatDau)
    {
        CanHoId = canHoId;
        UserId = userId;
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
        NgayBatDau = ngayBatDau;
        TrangThai = true;
    }

    public void ThayDoiLoaiQuanHe(int loaiQuanHeCuTruId)
    {
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
    }

    public void KetThucCuTru(DateTime ngayKetThuc)
    {
        NgayKetThuc = ngayKetThuc;
        TrangThai = false;
    }
}
