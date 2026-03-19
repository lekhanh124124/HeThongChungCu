using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class LaiChamTra : BaseEntity
{
    public int HoaDonId { get; private set; }
    public DateTime NgayTinh { get; private set; }
    public decimal SoTienGoc { get; private set; }
    public int SoNgayCham { get; private set; }
    public decimal LaiSuat { get; private set; }
    public decimal SoTienLai { get; private set; }

    private LaiChamTra() { } // EF Core

    public LaiChamTra(int hoaDonId, DateTime ngayTinh, decimal soTienGoc, int soNgayCham, decimal laiSuat, decimal soTienLai)
    {
        HoaDonId = hoaDonId;
        NgayTinh = ngayTinh;
        SoTienGoc = soTienGoc;
        SoNgayCham = soNgayCham;
        LaiSuat = laiSuat;
        SoTienLai = soTienLai;
    }
}
