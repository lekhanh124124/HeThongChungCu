namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoExcelTemplateDto
{
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public string TenCanHo { get; set; } = null!;
    public string Block { get; set; } = null!;
    public string TenTang { get; set; } = null!;
    public int DichVuId { get; set; }
    public string TenDichVu { get; set; } = null!;
    public decimal ChiSoCu { get; set; }
    public string? MaTraCuu { get; set; }
}
