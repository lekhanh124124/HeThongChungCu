namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class LuaChonKhaoSatReadModel
{
    public int Id { get; set; }
    public int CauHoiKhaoSatId { get; set; }
    public string NoiDungLuaChon { get; set; } = string.Empty;
    public bool IsUngVienBQT { get; set; }
    public string? TieuSuUngVien { get; set; }
    public int? UngVienId { get; set; }
}
