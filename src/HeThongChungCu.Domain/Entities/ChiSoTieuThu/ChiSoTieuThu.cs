using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ChiSoTieuThu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int DichVuId { get; private set; }
    public decimal ChiSoCu { get; private set; }
    public decimal ChiSoMoi { get; private set; }
    public decimal SoLuong => ChiSoMoi - ChiSoCu;
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTimeOffset NgayChot { get; private set; }
    public bool IsLock { get; private set; }

    private ChiSoTieuThu() { } // EF Core

    public ChiSoTieuThu(int canHoId, int dichVuId, decimal chiSoCu, decimal chiSoMoi, int thang, int nam, DateTimeOffset ngayChot)
    {
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không thể nhỏ hơn chỉ số cũ.");

        CanHoId = canHoId;
        DichVuId = dichVuId;
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
        IsLock = false;
    }

    public void Update(decimal chiSoCu, decimal chiSoMoi, int thang, int nam, DateTimeOffset ngayChot)
    {
        if (IsLock)
            throw new BusinessException("Chỉ số tiêu thụ đã bị khóa, không thể cập nhật.");
        
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không thể nhỏ hơn chỉ số cũ.");

        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
    }

    public void Lock() => IsLock = true;
}
