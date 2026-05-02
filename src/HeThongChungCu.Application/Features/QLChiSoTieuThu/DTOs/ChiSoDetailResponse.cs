namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoDetailResponse : ChiSoResponse
{
    public string? GhiChu { get; set; }
    public int? AnhDongHoId { get; set; }
    public string? AnhDongHoUrl { get; set; }
    public int? HoaDonId { get; set; }
}
