using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ChiSoTieuThu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public LoaiDichVu LoaiDichVuId { get; private set; } = null!;
    public double ChiSo { get; private set; }
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTime NgayChot { get; private set; }
    public bool IsLock { get; private set; }

    private ChiSoTieuThu() { } // EF Core

    public ChiSoTieuThu(int canHoId, LoaiDichVu loaiDichVuId, double chiSo, int thang, int nam, DateTime ngayChot)
    {
        CanHoId = canHoId;
        LoaiDichVuId = loaiDichVuId;
        ChiSo = chiSo;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
        IsLock = false;
    }

    public void Update(double chiSo, int thang, int nam, DateTime ngayChot)
    {
        if (IsLock)
            throw new BusinessException("Chỉ số tiêu thụ đã bị khóa, không thể cập nhật.");

        ChiSo = chiSo;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
    }

    public void Lock()
    {
        IsLock = true;
    }
}
