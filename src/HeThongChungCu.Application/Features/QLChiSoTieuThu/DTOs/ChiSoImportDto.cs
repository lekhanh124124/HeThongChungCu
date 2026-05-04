namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoImportDto
{
    public int CanHoId { get; set; }
    public string? MaCanHo { get; set; }
    public int DichVuId { get; set; }
    public string? TenDichVu { get; set; }
    public decimal ChiSoCu { get; set; }
    public decimal SoMoi { get; set; }
    public string? GhiChu { get; set; }
    public string? MaTraCuu { get; set; }
}
