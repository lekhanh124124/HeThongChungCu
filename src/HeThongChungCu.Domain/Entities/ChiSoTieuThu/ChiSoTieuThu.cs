using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ChiSoTieuThu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int DichVuId { get; private set; }
    public double ChiSoCu { get; private set; }
    public double ChiSoMoi { get; private set; }
    public double SoLuong => ChiSoMoi - ChiSoCu;
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTime NgayChot { get; private set; }
    public bool IsLock { get; private set; }

    private ChiSoTieuThu() { } // EF Core

    public ChiSoTieuThu(int canHoId, int dichVuId, double chiSoCu, double chiSoMoi, int thang, int nam, DateTime ngayChot)
    {
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không được nhỏ hơn chỉ số cũ.");

        CanHoId = canHoId;
        DichVuId = dichVuId;
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
        IsLock = false;
    }

    public void Update(double chiSoCu, double chiSoMoi, int thang, int nam, DateTime ngayChot)
    {
        if (IsLock)
            throw new BusinessException("Chỉ số tiêu thụ đã bị khóa, không thể cập nhật.");
        
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không được nhỏ hơn chỉ số cũ.");

        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
    }

    public void Lock() => IsLock = true;
}
