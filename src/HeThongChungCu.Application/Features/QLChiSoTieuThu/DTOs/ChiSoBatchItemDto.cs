namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoBatchItemDto
{
    public int CanHoId { get; set; }
    public int DichVuId { get; set; }
    public decimal ChiSoCu { get; set; }
    public decimal ChiSoMoi { get; set; }
    public int? AnhDongHoId { get; set; }
    public string? GhiChu { get; set; }
}
