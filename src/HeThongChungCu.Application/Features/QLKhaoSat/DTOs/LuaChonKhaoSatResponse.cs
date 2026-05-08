namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class LuaChonKhaoSatResponse
{
    public int Id { get; set; }
    public string NoiDungLuaChon { get; set; } = string.Empty;
    public bool IsUngVienBQT { get; set; }
    public string? TieuSuUngVien { get; set; }
    public int? UngVienId { get; set; }
}
