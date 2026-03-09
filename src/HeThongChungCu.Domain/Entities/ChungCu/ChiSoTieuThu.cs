using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class ChiSoTieuThu : AuditableEntity
{
    public int CanHoId { get; private set; }
    public int LoaiDichVuId { get; private set; }
    public double ChiSoCu { get; private set; }
    public double ChiSoMoi { get; private set; }
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTime NgayChot { get; private set; }

    private ChiSoTieuThu() { } // EF Core

    public ChiSoTieuThu(int canHoId, int loaiDichVuId, double chiSoCu, double chiSoMoi, int thang, int nam, DateTime ngayChot)
    {
        CanHoId = canHoId;
        LoaiDichVuId = loaiDichVuId;
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
    }

    public void Update(double chiSoCu, double chiSoMoi, int thang, int nam, DateTime ngayChot)
    {
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayChot = ngayChot;
    }
}
