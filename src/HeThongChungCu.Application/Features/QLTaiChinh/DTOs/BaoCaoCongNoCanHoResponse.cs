namespace HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

public class BaoCaoCongNoCanHoResponse
{
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public string TenToaNha { get; set; } = null!;
    public string TenTang { get; set; } = null!;
    public string TenChuHo { get; set; } = null!;
    public decimal NoDauKy { get; set; }
    public decimal PhatSinhTrongKy { get; set; }
    public decimal DaThanhToanTrongKy { get; set; }
    public decimal NoCuoiKy { get; set; }
}
