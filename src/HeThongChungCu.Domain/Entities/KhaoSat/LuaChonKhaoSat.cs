using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class LuaChonKhaoSat : AuditableEntity
{
    public int CauHoiKhaoSatId { get; private set; }
    public CauHoiKhaoSat CauHoiKhaoSat { get; private set; } = null!;
    public string NoiDungLuaChon { get; private set; } = null!;
    
    // Thuộc tính mở rộng cho Bầu cử Ban Quản trị chung cư
    public bool IsUngVienBQT { get; private set; }
    public string? TieuSuUngVien { get; private set; }
    public int? UngVienId { get; private set; }

    private LuaChonKhaoSat() : base() { } // EF Core

    private LuaChonKhaoSat(string noiDung, bool isUngVien, string? tieuSu, int? ungVienId)
    {
        NoiDungLuaChon = noiDung;
        IsUngVienBQT = isUngVien;
        TieuSuUngVien = tieuSu;
        UngVienId = ungVienId;
    }

    public static LuaChonKhaoSat Create(string noiDung, bool isUngVien = false, string? tieuSu = null, int? ungVienId = null)
    {
        return new LuaChonKhaoSat(noiDung, isUngVien, tieuSu, ungVienId);
    }
}
