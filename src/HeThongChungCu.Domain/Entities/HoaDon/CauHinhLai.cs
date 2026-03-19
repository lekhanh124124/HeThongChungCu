using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class CauHinhLai : AggregateRoot
{
    public string MaCauHinh { get; private set; } = string.Empty;
    public decimal LaiSuatThang { get; private set; }
    public int SoNgayChoPhep { get; private set; }
    public int NguongQuaHanNhe { get; private set; }
    public int NguongQuaHanNang { get; private set; }
    public DateTime NgayApDung { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }

    private CauHinhLai() { } // EF Core

    public CauHinhLai(
        string maCauHinh, 
        decimal laiSuatThang, 
        int soNgayChoPhep, 
        int nguongQuaHanNhe, 
        int nguongQuaHanNang, 
        DateTime ngayApDung, 
        DateTime? ngayKetThuc = null)
    {
        MaCauHinh = maCauHinh;
        LaiSuatThang = laiSuatThang;
        SoNgayChoPhep = soNgayChoPhep;
        NguongQuaHanNhe = nguongQuaHanNhe;
        NguongQuaHanNang = nguongQuaHanNang;
        NgayApDung = ngayApDung;
        NgayKetThuc = ngayKetThuc;
    }

    public bool IsActive(DateTime date)
    {
        return date >= NgayApDung && (NgayKetThuc == null || date <= NgayKetThuc.Value);
    }
}
