using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class TraLoiPhanAnh : AuditableEntity
{
    public int YeuCauPhanAnhId { get; private set; }
    public YeuCauPhanAnh YeuCauPhanAnh { get; private set; } = null!;
    public string NoiDung { get; private set; } = null!;
    public bool IsNhanVien { get; private set; } // Phân biệt Nhân viên (true) hay Cư dân (false) gửi

    private TraLoiPhanAnh() : base() { } // EF Core

    private TraLoiPhanAnh(string noiDung, bool isNhanVien)
    {
        NoiDung = noiDung;
        IsNhanVien = isNhanVien;
    }

    public static TraLoiPhanAnh Create(string noiDung, bool isNhanVien)
    {
        return new TraLoiPhanAnh(noiDung, isNhanVien);
    }
}
